using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using SDON.Model;

namespace SDON.Serialization
{
    /// <summary>
    /// Abstract class that contains the implementation of the SDON serializer. The base class for all SDON objects.
    /// </summary>
    [KnownType("GetKnownTypes")]
    [Serializable]
    public abstract class SDONSerializeable : ISerializable
    {
        protected bool _hasBeenValidated = false;
        protected bool _hasBeenModified = false;

        /// <summary>
        /// The types included in the runtime serialization. Generated Ad-hoc upon serialization.
        /// </summary>
        private static List<Type> _knownTypes = null;

        /// <summary>
        /// Cache of the lists of all the fields/properties that have a DataMemberAttribute assoicated with them that belong to the given type.
        /// </summary>
        private static Dictionary<Type, List<MemberInfo>> _memberStore = new Dictionary<Type, List<MemberInfo>>();

        /// <summary>
        /// Cache of the relevant attributes that belong to each member of an object that can be serialized.
        /// </summary>
        private static Dictionary<MemberInfo, Attributes> _attributeStore = new Dictionary<MemberInfo, Attributes>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal SDONSerializeable()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal SDONSerializeable(SerializationInfo info, StreamingContext context)
        {
            Deserialize(info, ref context);
        }

        /// <summary>
        /// Overridable deserialization function. Walks the list of serializeable members and deserializes them. Override to provide special serialization logic for a given member.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        protected virtual void Deserialize(SerializationInfo info, ref StreamingContext context)
        {
            List<MemberInfo> serializeableMembers = GetSerializeableMembers();
            if (serializeableMembers == null) return;

            foreach (MemberInfo curMember in serializeableMembers)
            {
                DeserializeMember(curMember, info, ref context);
            }
        }

        /// <summary>
        /// Deserializes a member and assigns its value to the current object if possible.
        /// </summary>
        /// <param name="member">The current property or field being set.</param>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        /// <returns></returns>
        protected bool DeserializeMember(MemberInfo member, SerializationInfo info, ref StreamingContext context)
        {
            if (member == null || info == null) return false;

            Attributes relevantAttrs = null;
            if (_attributeStore.TryGetValue(member, out relevantAttrs) == false) return false;

            string memberName = member.Name;
            //if the name is overridden in the data member, use that name instead
            if (String.IsNullOrWhiteSpace(relevantAttrs.DataMember.Name) == false) memberName = relevantAttrs.DataMember.Name;

            //check to make sure the value being deserialized is in the incoming object graph
            if (IsMemberPresent(memberName, info) == false) return false;

            //set the value. If the value does not exist in the object that is being deserialized, an exception will be thrown
            try
            {
                Action<object, object> setter = null;
                Type memberType = null;
                object memberValue = null;

                if (member is PropertyInfo)
                {
                    PropertyInfo propInfo = (PropertyInfo)member;
                    if (propInfo.CanWrite == false) return false;

                    memberType = propInfo.PropertyType;
                    setter = propInfo.SetValue;
                }
                else if (member is FieldInfo)
                {
                    FieldInfo fieldInfo = (FieldInfo)member;

                    memberType = fieldInfo.FieldType;
                    setter = fieldInfo.SetValue;
                }

                //if the member is a list of a generic type, the serializer won't give us the deserialized value (it throws an exception), so we have to do some special logic to deserialize it properly
                if (memberType.GetInterface("IList") != null && memberType.IsGenericType == true && memberType.GetGenericArguments().Any(t => t == typeof (SDONSerializeable) || t.IsSubclassOf(typeof(SDONSerializeable))))
                {
                    //first we have to get the object out, and the only thing that makes the SerializationInfo's GetValue method happy is if we take out the value as an object[]
                    memberValue = info.GetValue(memberName, typeof(Object[]));

                    //then make the actual generic type list to re-fill with the contents of our object array
                    object deserialized = Activator.CreateInstance(memberType);
                    foreach (var item in (memberValue as Object[]))
                    {
                        (deserialized as IList).Add(item); //add each item to the list
                    }

                    //then set the memberValue to be stuffed back into the object as our freshly instantiated and deserialized list
                    memberValue = deserialized;
                }
                else //regular object, normal behavior works fine
                {
                    memberValue = info.GetValue(memberName, memberType);
                }

                //set the field or property value
                setter(this, memberValue);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the incoming object graph has a value with the given name.
        /// </summary>
        /// <param name="memberName">The name of the value to find.</param>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <returns></returns>
        protected bool IsMemberPresent(string memberName, SerializationInfo info)
        {
            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == memberName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Populates the SerializationInfo object with all the values from the current object that are not set to their default values.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are going to be written into JSON) of the object being serialized.</param>
        /// <param name="context">The serialization context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            List<MemberInfo> members = GetSerializeableMembers();
            if (members == null) return;

            List<string> usedNames = new List<string>();

            Serialize(members, info, ref context, ref usedNames);
        }

        /// <summary>
        /// Overridable serialization function. Writes out all the properties with non-default values to the serialization info. Can be overridden if specialized serialization logic is required.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="info">Serialization info (all the values that are going to be written into JSON) of the object being serialized.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="usedPropertyNames">A list of property names that have already been used. An exception is thrown if a name is attempted to be used twice.</param>
        protected virtual void Serialize(List<MemberInfo> members, SerializationInfo info, ref StreamingContext context, ref List<string> usedPropertyNames)
        {
            foreach (MemberInfo curMember in members)
            {
                SerializeMember(curMember, info, ref context, ref usedPropertyNames);
            }
        }

        /// <summary>
        /// Serializes a field or property and writes it to the serialization info object.
        /// </summary>
        /// <param name="member">The property or field being serialized.</param>
        /// <param name="info">Serialization info (all the values that are going to be written into JSON) of the object being serialized.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="usedPropertyNames">A list of property names that have already been used. An exception is thrown if a name is attempted to be used twice.</param>
        /// <returns></returns>
        protected bool SerializeMember(MemberInfo member, SerializationInfo info, ref StreamingContext context, ref List<string> usedPropertyNames)
        {
            if (member == null || info == null) return false;

             Attributes relevantAttrs = null;
            if (_attributeStore.TryGetValue(member, out relevantAttrs) == false) return false;
                
            string serializeableName = member.Name;

            //override serializeableName with data member's name if one was provided
            if (String.IsNullOrWhiteSpace(relevantAttrs.DataMember.Name) == false) serializeableName = relevantAttrs.DataMember.Name;

            //name was already used, cant serialize it again
            if (usedPropertyNames != null && usedPropertyNames.IndexOf(serializeableName) != -1) throw new Exception("DataMember name \"" + serializeableName + "\" is a duplicate in type \"" + GetType().ToString() + "\". Property names must be unique within their containing type.");
            usedPropertyNames.Add(serializeableName);

            object serializeableValue = null;
            Type serializeableType = null;

            //get the value we're going to serialize
            if (member is FieldInfo)
            {
                FieldInfo fieldInfo = (FieldInfo)member;

                serializeableValue = fieldInfo.GetValue(this);
                serializeableType = fieldInfo.FieldType;
            }
            else if (member is PropertyInfo)
            {
                PropertyInfo propInfo = (PropertyInfo)member;
                if (propInfo.CanRead == false) return false; //cant get the value of the property, don't serialize it

                serializeableValue = propInfo.GetValue(this);
                serializeableType = propInfo.PropertyType;
            }

            //check to see if we should skip serializing this value if a ignore if default attribute was attached
            if (relevantAttrs.IgnoreIfValue != null)
            {
                if (relevantAttrs.IgnoreIfValue.Default == null && serializeableValue == null) return false; //null is the default value and the value to serialize is null, dont serialize it
                if (relevantAttrs.IgnoreIfValue.Default != null && relevantAttrs.IgnoreIfValue.Default.Equals(serializeableValue) == true) return false; //value equals our "skip if this value" value, dont serialize it
                if (serializeableValue is IList && serializeableValue.GetType().IsGenericType == true && (serializeableValue as IList).Count == 0) return false; // don't serialize empty lists
                if (serializeableValue is Array && (serializeableValue as Array).Length == 0) return false; //dont serialize empty arrays
            }


            info.AddValue(serializeableName, serializeableValue, serializeableType);

            return true;
        }

        /// <summary>
        /// Gets all the fields belonging to the object.
        /// </summary>
        /// <returns></returns>
        protected FieldInfo[] GetFields()
        {
            var type = GetType();
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// Gets all the properties belonging to the object.
        /// </summary>
        /// <returns></returns>
        protected PropertyInfo[] GetProperties()
        {
            var type = GetType();
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// Either gets the list of fields or properties that can be serialized from the attribute/member store or uses reflection to generate them and then stores them in the attribute/member store.
        /// </summary>
        /// <returns></returns>
        protected List<MemberInfo> GetSerializeableMembers()
        {
            Type t = GetType();

            //see if we already have the field/property infos, if we do return them
            if (_memberStore.ContainsKey(t) == true)
            {
                return _memberStore[t];
            }

            //do not have field/property infos yet, go get them
            FieldInfo[] fieldInfos = GetFields();
            PropertyInfo[] propInfos = GetProperties();

            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(fieldInfos);
            members.AddRange(propInfos);

            var serializeableMembers = new List<MemberInfo>();

            //loop through all the fields/properties looking for ones that have the DataMememberAttribute attribute and add them to the list of serializeable members
            foreach (MemberInfo curMember in members)
            {
                DataMemberAttribute dataMember = null;
                IgnoreIfDefaultValueAttribute ignoreIfValue = null;

                List<Attribute> attrs = new List<Attribute>(curMember.GetCustomAttributes());
                foreach (Attribute curAttr in attrs)
                {
                    if (curAttr is DataMemberAttribute)
                    {
                        dataMember = (DataMemberAttribute)curAttr;
                    }
                    else if (curAttr is IgnoreIfDefaultValueAttribute)
                    {
                        ignoreIfValue = (IgnoreIfDefaultValueAttribute)curAttr;
                    }

                    if (ignoreIfValue != null && dataMember != null) break;
                }

                //no DataMemberAttribute for this field, shouldn't serialize it
                if (dataMember == null) continue;
                
                //add to our list of members that can be serialized
                serializeableMembers.Add(curMember);

                //make the record of the relevant attributes so we dont have to go looping through the attribute list again for them
                Attributes relevantAttrs = new Attributes();
                relevantAttrs.DataMember = dataMember;
                relevantAttrs.IgnoreIfValue = ignoreIfValue;

                //add to the dictionary paring a member info to its relevant attributes
                if (_attributeStore.ContainsKey(curMember) == false) _attributeStore.Add(curMember, relevantAttrs);
            }

            _memberStore.Add(t, serializeableMembers);
            return serializeableMembers;
        }

        /// <summary>
        /// Helps the serializer resolve types as it serializes the SDON object graph.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Type> GetKnownTypes()
        {
            if (_knownTypes == null)
            {
                //get every type from the model namespace
                _knownTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "SDON.Model" && t.GetCustomAttribute(typeof(SerializableAttribute)) != null).ToList();

                //then get the List<T> version of each type
                List<Type> listTypes = new List<Type>();
                foreach (Type t in _knownTypes)
                {
                    listTypes.Add(typeof(List<>).MakeGenericType(t));
                }

                _knownTypes.AddRange(listTypes);
            }

            return _knownTypes;
        }

        /// <summary>
        /// Class for containing the attributes that are relevant to our serialization.
        /// </summary>
        private class Attributes
        {
            public DataMemberAttribute DataMember = null;
            public IgnoreIfDefaultValueAttribute IgnoreIfValue = null;
        }
    }
}

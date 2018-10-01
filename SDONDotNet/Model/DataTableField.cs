using SDON.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SDON.Model
{
    /// <summary>
    /// Object representing  value in a row in a data table;
    /// </summary>
    [Serializable]
    public sealed class DataTableField : SDONSerializeable
    {
        [DataMember(Name = "Name")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _name = null;

        [DataMember(Name = "Value")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _value = null;

        /// <summary>
        /// The name of the corresponding column to add a value to.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The value to give the row.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataTableField()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal DataTableField(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

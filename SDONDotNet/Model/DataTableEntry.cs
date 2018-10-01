using SDON.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SDON.Model
{
    [Serializable]
    public sealed class DataTableEntry : SDONSerializeable
    {
        [IgnoreIfDefaultValue(Default = null)]
        [DataMember(Name = "ID")]
        private string _id = null;

        [IgnoreIfDefaultValue(Default = null)]
        [DataMember(Name = "TableName")]
        private string _tableName = null;

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataTableEntry()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal DataTableEntry(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}

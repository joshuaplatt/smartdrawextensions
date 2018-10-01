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
    public sealed class DataTable : SDONSerializeable
    {
        [DataMember(Name = "ID")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _id = null;

        [DataMember(Name = "TableName")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _tableName = null;

        [DataMember(Name = "Columns")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<DataTableColumn> _columns = null;

        [DataMember(Name = "Rows")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<DataTableRow> _rows = null;

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

        public List<DataTableColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public List<DataTableRow> Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataTable()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal DataTable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

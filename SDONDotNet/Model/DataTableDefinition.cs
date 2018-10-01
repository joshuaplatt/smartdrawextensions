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
    /// Object for containing a table of custom data for a shape, defines the schema of a table and gives it an ID so it can be used in shapes contained in the diagram.
    /// </summary>
    [Serializable]
    public sealed class DataTableDefinition : SDONSerializeable
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

        /// <summary>
        /// The unique identifier for the data table so it can be referenced in a shape object.
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The name to give the table.
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// The list of column defintions in a data table. The schema of the data table.
        /// </summary>
        public List<DataTableColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        /// <summary>
        /// Optional. The data to populate rows in the data table with.
        /// </summary>
        public List<DataTableRow> Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataTableDefinition()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal DataTableDefinition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

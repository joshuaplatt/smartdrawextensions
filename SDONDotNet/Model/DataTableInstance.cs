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
    /// Object that contains a reference to a DataTable defined in the Diagram object. Used to fill out the schema defined in the DataTable array in the Diagram object.
    /// </summary>
    [Serializable]
    public sealed class DataTableInstance : SDONSerializeable
    {
        [IgnoreIfDefaultValue(Default = null)]
        [DataMember(Name = "TableName")]
        private string _tableName = null;

        [IgnoreIfDefaultValue(Default = null)]
        [DataMember(Name = "Rows")]
        private List<DataTableRow> _rows = null;

        /// <summary>
        /// The name of a predefined table in the template used.
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// The rows to insert in the pre-existing table.
        /// </summary>
        public List<DataTableRow> Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataTableInstance()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal DataTableInstance(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}

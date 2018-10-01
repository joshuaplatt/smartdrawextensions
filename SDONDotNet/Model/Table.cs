using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SDON.Serialization;

namespace SDON.Model
{
    /// <summary>
    /// Object that represents a table that has been inserted into a shape.
    /// </summary>
    [Serializable]
    public sealed class Table : SDONSerializeable
    {
        [DataMember(Name = "Rows")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _rows = -1;

        [DataMember(Name = "Columns")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _columns = -1;

        [DataMember(Name = "ColumnWidth")]
        [IgnoreIfDefaultValue(Default = 100)]
        private int _columnWidth = 100;

        [DataMember(Name = "RowHeight")]
        [IgnoreIfDefaultValue(Default = 50)]
        private int _rowHeight = 50;

        [DataMember(Name = "Cells")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<Cell> _cell = null;

        [DataMember(Name = "AlternateRows")]
        [IgnoreIfDefaultValue(Default = null)]
        private TableAlternateRowsColors _alternateRows = null;

        [DataMember(Name = "Join")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<Join> _join = null;

        [DataMember(Name = "ColumnProperties")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<ColumnProperties> _columnProperties = null;

        [DataMember(Name = "RowProperties")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<RowProperties> _rowProperties = null;

        /// <summary>
        /// The number of rows in the table. This can be omitted for a default of 1 if there are columns defined. If neither the number of  rows or columns is defined, the Table object is ignored.
        /// </summary>
        public int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        /// <summary>
        /// The number of columns in the table. This can be omitted for a default of 1 if there are rows defined. If neither the number of  rows or columns is defined, the Table object is ignored.
        /// </summary>
        public int Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        /// <summary>
        /// The width of the column in 1/100th inches.
        /// </summary>
        public int ColumnWidth
        {
            get { return _columnWidth; }
            set { _columnWidth = value; }
        }

        /// <summary>
        /// The Height of the Row in 1/100th inches.
        /// </summary>
        public int RowHeight
        {
            get { return _rowHeight; }
            set { _rowHeight = value; }
        }

        /// <summary>
        /// Specific properties for individual cells.
        /// </summary>
        public List<Cell> Cell
        {
            get { return _cell; }
            set { _cell = value; }
        }

        /// <summary>
        /// Sets up a color scheme for alternating row colors in a table.
        /// </summary>
        public TableAlternateRowsColors AlternateRows
        {
            get { return _alternateRows; }
            set { _alternateRows = value; }
        }
       
        /// <summary>
        /// List of elements that define a range of table cells to join into one cell.
        /// </summary>
        public List<Join> Join
        {
            get { return _join; }
            set { _join = value; }
        }

        /// <summary>
        /// Special properties of individual columns.
        /// </summary>
        public List<ColumnProperties> ColumnProperties
        {
            get { return _columnProperties; }
            set { _columnProperties = value; }
        }

        /// <summary>
        /// Special properties of individual rows.
        /// </summary>
        public List<RowProperties> RowProperties
        {
            get { return _rowProperties; }
            set { _rowProperties = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Table(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

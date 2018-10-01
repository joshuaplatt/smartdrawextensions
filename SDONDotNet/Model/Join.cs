using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SDON.Serialization;

namespace SDON.Model
{
    [Serializable]
    public sealed class Join : SDONSerializeable
    {
        [DataMember(Name = "Row")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _row = -1;

        [DataMember(Name = "Column")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _column = -1;

        [DataMember(Name = "N")]
        [IgnoreIfDefaultValue(Default = 1)]
        private int _n = 1;

        [DataMember(Name = "Down")]
        [IgnoreIfDefaultValue(Default = 1)]
        private int _down = 1;

        /// <summary>
        /// The row of the first cell to be joined to others to the right or below it. Note that the first row is row 1 not row 0.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        /// <summary>
        /// The column of the first cell to be joined to others to the right or below it. Note that the first column is column 1 not column 0.
        /// </summary>
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        /// <summary>
        /// The number of cells to join to the first cell. If this exceeds the number of cells available within a row or column the number is reduced to the maximum possible. The default value is 1.
        /// </summary>
        public int N
        {
            get { return _n; }
            set { _n = value; }
        }

        /// <summary>
        /// Adding this parameter with a non zero value makes the join happen down the column
        /// </summary>
        public int Down
        {
            get { return _down; }
            set { _down = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Join()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Join(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

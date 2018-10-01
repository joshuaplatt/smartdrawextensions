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
    /// Represents an array of shapes arranged in a grid.
    /// </summary>
    [Serializable]
    public sealed class ShapeArray : SDONSerializeable
    {
        [DataMember(Name = "Shapes")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<Shape> _shapes = null;

        [DataMember(Name = "Arrangement")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _arrangement = null;

        [DataMember(Name = "VerticalSpacing")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _verticalSpacing = -1.0;

        [DataMember(Name = "HorizontalSpacing")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _horizontalSpacing = -1.0;

        [DataMember(Name = "Wrap")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _wrap = -1;

        [DataMember(Name = "ArrayAlignH")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _arrayAlignH = null;

        [DataMember(Name = "ArrayAlignV")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _arrayAlignV = null;

        /// <summary>
        /// The array of shapes to include in the group.
        /// </summary>
        public List<Shape> Shapes
        {
            get { return _shapes; }
            set {_shapes = value;}
        }

        /// <summary>
        /// The way shapes are arranged in a group. Must be a value from ShapeArrangementTypes.
        /// </summary>
        public string Arrangement
        {
            get { return _arrangement; }
            set { _arrangement = value; }
        }

        /// <summary>
        /// The spacing between columns in 1/100".
        /// </summary>
        public double VerticalSpacing
        {
            get { return _verticalSpacing; }
            set { _verticalSpacing = value; }
        }

        /// <summary>
        /// The spacing between rows in 1/100".
        /// </summary>
        public double HorizontalSpacing
        {
            get { return _horizontalSpacing; }
            set { _horizontalSpacing = value; }
        }

        /// <summary>
        /// The maximum number of rows for a "Row" arrangement before it wraps to a new row, or the maximum number of columns for a Column arrangement before it wraps to a new column.
        /// </summary>
        public int Wrap
        {
            get { return _wrap; }
            set { _wrap = value; }
        }

        /// <summary>
        /// Controls the positioning of a shape in a column of shapes. Must be a value from HorizontalAlignments.
        /// </summary>
        public string ArrayAlignH
        {
            get { return _arrayAlignH; }
            set { _arrayAlignH = value; }
        }

        /// <summary>
        /// Controls the positioning of a shape in a row of shapes. Must be a value from VerticalAlignments.
        /// </summary>
        public string ArrayAlignV
        {
            get { return _arrayAlignV; }
            set { _arrayAlignV = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShapeArray()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal ShapeArray(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}

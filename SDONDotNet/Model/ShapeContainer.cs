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
    /// An object for holding shapes in a grid-like pattern.
    /// </summary>
    [Serializable]
    public sealed class ShapeContainer : SDONSerializeable
    {
        [DataMember(Name = "Shapes")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<Shape> _shapes = null;

        [DataMember(Name = "Arrangement")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _arrangement = null;

        [DataMember(Name = "Wrap")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _wrap = -1;

        [DataMember(Name = "VerticalSpacing")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _verticalSpacing = -1;

        [DataMember(Name = "HorizontalSpacing")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _horizontalSpacing = -1;

        [DataMember(Name = "ShapesAlignH")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _shapesAlignH = null;

        [DataMember(Name = "ShapesAlignV")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _shapesAlignV = null;

        [DataMember(Name = "Hide")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _hide = null;

        [DataMember(Name = "DefaultShape")]
        [IgnoreIfDefaultValue(Default = null)]
        private Shape _defaultShape = null;

        /// <summary>
        /// The shapes contained by the ShapeContainer.
        /// </summary>
        public List<Shape> Shapes
        {
            get { return _shapes; }
            set { _shapes = value; }
        }

        /// <summary>
        /// The arrangement grid pattern. Must be a value from ShapeContainerArrangement.
        /// </summary>
        public string Arrangement
        {
            get { return _arrangement; }
            set { _arrangement = value; }
        }

        /// <summary>
        /// This is the maximum number of rows for a "Row" arrangement before it wraps to a new row, or the maximum number of columns for a Column arrangement before it wraps to a new column.
        /// </summary>
        public int Wrap
        {
            get { return _wrap; }
            set { _wrap = value; }
        }

        /// <summary>
        /// Spacing between rows in 1/100". This is inherited by child ShapeContainers.
        /// </summary>
        public int VerticalSpacing
        {
            get { return _verticalSpacing; }
            set { _verticalSpacing = value; }
        }

        /// <summary>
        /// Spacing between columns in 1/100". This is inherited by child ShapeContainers.
        /// </summary>
        public int HorizontalSpacing
        {
            get { return _horizontalSpacing; }
            set { _horizontalSpacing = value; }
        }

        /// <summary>
        /// Controls the positioning of shapes in a column of shapes. Must be a value from ShapeAlignHorizontal.
        /// </summary>
        public string ShapesAlignH
        {
            get { return _shapesAlignH; }
            set { _shapesAlignH = value; }
        }

        /// <summary>
        /// Controls the positioning of a shape in a row of shapes. Must be a value from ShapeAlignVertical
        /// </summary>
        public string ShapesAlignV
        {
            get { return _shapesAlignV; }
            set { _shapesAlignV = value; }
        }

        /// <summary>
        /// Causes the parent to be deleted after the layout is achieved. It is ignored if there is no ShapeContainer.
        /// </summary>
        public bool Hide
        {
            get
            {
                if (_hide.HasValue)
                {
                    return _hide.Value;
                }
                else
                {
                    return false;
                }
            }
            set { _hide = value; }
        }

        /// <summary>
        /// The default shape properties of all shapes in the container.
        /// </summary>
        public Shape DefaultShape
        {
            get { return _defaultShape; }
            set { _defaultShape = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShapeContainer()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal ShapeContainer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}

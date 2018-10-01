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
    /// The properties of a column.
    /// </summary>
    [Serializable]
    public sealed class ColumnProperties : SDONSerializeable
    {
        [DataMember(Name = "Index")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _index = -1;

        [DataMember(Name = "LineThick")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _lineThick = -1.0;

        [DataMember(Name = "LineColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _lineColor = null;

        [DataMember(Name = "Height")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _height = -1.0;

        [DataMember(Name = "Width")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _width = -1.0;

        [DataMember(Name = "FixedWidth")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _fixedWidth = null;

        /// <summary>
        /// The index of the column.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// The thickness of the column borders in 1/100”. Otherwise the thickness is the default for the template.
        /// </summary>
        public double LineThick
        {
            get { return _lineThick; }
            set { _lineThick = value; }
        }

        /// <summary>
        /// The line color of the column borders as a hex RGB value. If omitted, the color is the default for the template.
        /// </summary>
        public string LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// The minimum height of the column in 1/100”. The text in cells in the row may force the row to be taller than this height.
        /// </summary>
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// The desired width of the column in 1/100”. Note any change n column N’s width reduces the width of column N+1 by the same amount.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Makes the row not scale as the shape that contains the table grows.
        /// </summary>
        public bool FixedWidth
        {
            get { return _fixedWidth == null ? false : (bool)_fixedWidth; }
            set { _fixedWidth = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnProperties()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal ColumnProperties(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

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
    /// Object for containing the properties of a row in a table.
    /// </summary>
    [Serializable]
    public sealed class RowProperties : SDONSerializeable
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

        [DataMember(Name = "LinePattern")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _linePattern = null;

        [DataMember(Name = "Height")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _height = -1.0;

        [DataMember(Name = "FixedHeight")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _fixedHeight = null;

        /// <summary>
        /// The index of the row.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// The thickness of the row borders in 1/100”. If omitted, the thickness is the default for the template.
        /// </summary>
        public double LineThick
        {
            get { return _lineThick; }
            set { _lineThick = value; }
        }

        /// <summary>
        /// The line color of the row borders as a hex RGB value or color alias. If omitted, the color is the default for the template.
        /// </summary>
        public string LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// The pattern of the line for the row border. Must be a value from the LinePatterns enum.
        /// </summary>
        public string LinePattern
        {
            get { return _linePattern; }
            set { _linePattern = value; }
        }

        /// <summary>
        /// The minimum height of the row in 1/100”. The text in cells in the row may force the row to be taller than this height.
        /// </summary>
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Makes the row not scale as the shape that contains the table grows.
        /// </summary>
        public bool FixedHeight
        {
            get { return _fixedHeight == null ? false : (bool)_fixedHeight; }
            set { _fixedHeight = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RowProperties()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal RowProperties(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

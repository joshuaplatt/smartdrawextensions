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
    /// A segmented line that links two shapes
    /// </summary>
    [Serializable]
    public sealed class Return : SDONSerializeable
    {
        [DataMember(Name = "StartID")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _startID = -1;

        [DataMember(Name = "EndID")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _endID = -1;

        [DataMember(Name = "StartDirection")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _startDirection = null;

        [DataMember(Name = "EndDirection")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _endDirection = null;

        [DataMember(Name = "LinePattern")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _linePattern = null;

        [DataMember(Name = "Label")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _label = null;

        [DataMember(Name = "Arrowhead")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _arrowhead = -1;

        [DataMember(Name = "StartArrow")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _startArrow = -1;

        [DataMember(Name = "EndArrow")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _endArrow = -1;

        [DataMember(Name = "LineThick")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _lineThick = -1.0;

        [DataMember(Name = "LineColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _lineColor = null;

        [DataMember(Name = "Curved")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _curved = null;

        /// <summary>
        /// The ID defined for the starting shape by its “ID” property.
        /// </summary>
        public int StartID
        {
            get { return _startID; }
            set { _startID = value; }
        }

        /// <summary>
        /// The ID defined for the ending shape by its “ID” property (see above). The starting and ending shapes define the direction of the line as far as any arrowhead is concerned, the arrowhead touches the ending shape.
        /// </summary>
        public int EndID
        {
            get { return _endID; }
            set { _endID = value; }
        }

        /// <summary>
        /// The direction the connector leaves the parent shape. The default is Down. A value from the Directions enum.
        /// </summary>
        public string StartDirection
        {
            get { return _startDirection; }
            set { _startDirection = value; }
        }

        /// <summary>
        /// The direction the line takes out of the ending shape. The default is Down. A value from the Directions enum.
        /// </summary>
        public string EndDirection
        {
            get { return _endDirection; }
            set { _endDirection = value; }
        }

        /// <summary>
        /// The pattern of the line connecting the shapes. A value from the LinePatterns enum.
        /// </summary>
        public string LinePattern
        {
            get { return _linePattern; }
            set { _linePattern = value; }
        }

        /// <summary>
        /// The text that appears on the line.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// By default returns have an arrowhead touching the end shape. This property can br turned off by using 0, or it can change the arrowhead from the default.
        /// </summary>
        public int Arrowhead
        {
            get { return _arrowhead; }
            set { _arrowhead = value; }
        }

        /// <summary>
        /// The arrowhead that will appear on the beginning of the line.
        /// </summary>
        public int StartArrow
        {
            get { return _startArrow; }
            set { _startArrow = value; }
        }

        /// <summary>
        /// The arrowhead that will appear on the end of the line.
        /// </summary>
        public int EndArrow
        {
            get { return _endArrow; }
            set { _endArrow = value; }
        }

        /// <summary>
        /// The thickness of the line in 1/100”. Otherwise the thickness is the default for the template.
        /// </summary>
        public double LineThick
        {
            get { return _lineThick; }
            set { _lineThick = value; }
        }

        /// <summary>
        /// Make the line color of the connector the specified RGB value . Otherwise the color is the default for the template.
        /// </summary>
        public string LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// When set to true this creates a curved return.
        /// </summary>
        public bool Curved
        {
            get
            {
                if (_curved.HasValue == true)
                {
                    return _curved.Value;
                }
                else
                {
                    return false;
                }
            }

            set { _curved = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Return()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Return(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

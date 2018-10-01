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
    /// An object that contains an array of shapes that are connected to it by an automatic connector. Defines an automatic connector.
    /// </summary>
    [Serializable]
    public sealed class ShapeConnector : SDONSerializeable
    {
        [DataMember(Name = "Collapse")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _collapse = null;

        [DataMember(Name = "Direction")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _direction = null;

        [DataMember(Name = "FillColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _fillColor = null;

        [DataMember(Name = "LineThick")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _lineThick = -1.0;

        [DataMember(Name = "LineColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _lineColor = null;

        [DataMember(Name = "LinePattern")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _linePattern = null;

        [DataMember(Name = "Arrangement")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _arrangement = null;

        [DataMember(Name = "Shapes")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<Shape> _shapes = null;

        [DataMember(Name = "ShapeConnectorType")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _shapeConnectorType = null;

        [DataMember(Name = "StartArrow")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _startArrow = -1;

        [DataMember(Name = "EndArrow")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _endArrow = -1;

        [DataMember(Name = "TextFont")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textFont = null;

        [DataMember(Name = "TextBold")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _textBold = null;

        [DataMember(Name = "TextItalic")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _textItalic = null;

        [DataMember(Name = "TextUnderline")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _textUnderline = null;

        [DataMember(Name = "TextSize")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _textSize = -1.0;

        [DataMember(Name = "TextColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textColor = null;

        [DataMember(Name = "DefaultShape")]
        [IgnoreIfDefaultValue(Default = null)]
        private Shape _defaultShape = null;

        /// <summary>
        /// Whether or not to collapse (hide) the connector. The connector is collapsed initially. This applies only to tree-like diagrams (not flowcharts).
        /// </summary>
        public bool Collapse
        {
            get
            {
                if (_collapse.HasValue == false) return false;
                return _collapse.Value;
            }

            set { _collapse = value; }
        }

        /// <summary>
        /// The direction of the connector from the parent shape. 
        /// 
        /// For Mind Maps this can be Left or Right for the ShapeLists connected to the root shape. All other uses are ignored. The default is “Right”. Mind Maps ignore any more than two ShapeLists for the root shape and  any more than one for other shapes.
        /// 
        /// For Org Charts (trees) the first ShapeList connected to the root shape can be in any direction and this sets the direction of the tree. The default is “Down”. All other values are ignored. Org charts ignore any more than one ShapeList per shape.
        /// 
        /// For Flow Charts any shape can have multiple ShapeLists in any direction. If two or more ShapeLists attached to a single shape have the same direction, they are shown as a split path. The default direction for a ShapeList is “Right”.
        /// </summary>
        public string Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// The background color for text labels.
        /// </summary>
        public string FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        /// <summary>
        /// The thickness of the line in 1/100”. If omitted, the thickness is the default for the template.
        /// </summary>
        public double LineThick
        {
            get { return _lineThick; }
            set { _lineThick = value; }
        }

        /// <summary>
        /// The line color of the connector as a hex RGB value. If omitted, the color is the default for the template.
        /// </summary>
        public string LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// The pattern of the line for the connector. Must be a value from the LinePatterns enum.
        /// </summary>
        public string LinePattern
        {
            get { return _linePattern; }
            set { _linePattern = value; }
        }

        /// <summary>
        /// The arrangement pattern of the shapes on the connector. Must be a value from the ShapeConnectorArrangement enum.
        /// </summary>
        public string Arrangement
        {
            get { return _arrangement; }
            set { _arrangement = value; }
        }

        /// <summary>
        /// A list of shapes that are attached to the connector. The shapes are attached in the order they appear in in the list.
        /// </summary>
        public List<Shape> Shapes
        {
            get { return _shapes; }
            set { _shapes = value; }
        }

        /// <summary>
        /// Defines the way the shape connector will be formatted. If omitted, the template default is used.
        /// </summary>
        public string ShapeConnectorType
        {
            get { return _shapeConnectorType; }
            set { _shapeConnectorType = value; }
        }

        /// <summary>
        /// The arrowhead that will appear on the beginning of the line. Must be a value from the Arrowheads enum.
        /// </summary>
        public int StartArrow
        {
            get { return _startArrow; }
            set { _startArrow = value; }
        }

        /// <summary>
        /// The arrowhead that will appear on the end of the line. Must be a value from the Arrowheads enum.
        /// </summary>
        public int EndArrow
        {
            get { return _endArrow; }
            set { _endArrow = value; }
        }

        /// <summary>
        /// Makes the text label be bold with a value of true, not bold with false, otherwise, if omitted, boldness follows the template default.
        /// </summary>
        public bool TextBold
        {
            get
            {
                if (_textBold.HasValue == true)
                {
                    return _textBold.Value;
                }
                else
                {
                    return false;
                }

            }

            set { _textBold = value; }
        }

        /// <summary>
        /// Makes the text label be italic with a value of true, not italic with false, otherwise, if omitted, italic follows the template default.
        /// </summary>
        public bool TextItalic
        {
            get
            {
                if (_textItalic.HasValue == true)
                {
                    return _textItalic.Value;
                }
                else
                {
                    return false;
                }

            }

            set { _textItalic = value; }
        }

        /// <summary>
        /// Makes the text label be underlined with a value of true, not underlined with false, otherwise, if omitted, underlined follows the template default.
        /// </summary>
        public bool TextUnderline
        {
            get
            {
                if (_textUnderline.HasValue == true)
                {
                    return _textUnderline.Value;
                }
                else
                {
                    return false;
                }

            }

            set { _textUnderline = value; }
        }

        /// <summary>
        /// The point size of the text label the specified value. If omitted, the text size is the default for the template.
        /// </summary>
        public double TextSize
        {
            get { return _textSize; }
            set { _textSize = value; }
        }

        /// <summary>
        /// The font of the text label the specified value. If omitted, the font is the default for the template. Any font can be defined, but will fall back to system default if font is unavailable.
        /// </summary>
        public string TextFont
        {
            get { return _textFont; }
            set { _textFont = value; }
        }

        /// <summary>
        /// Sets the default properties of any child shapes of the connector.
        /// </summary>
        public Shape DefaultShape
        {
            get { return _defaultShape; }
            set { _defaultShape = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShapeConnector()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal ShapeConnector(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

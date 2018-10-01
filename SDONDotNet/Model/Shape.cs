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
    /// Represents a shape in a SmartDraw diagram.
    /// </summary>
    [Serializable]
    public sealed class Shape : SDONSerializeable
    {   
        [DataMember(Name = "ID")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _id = -1;

        [DataMember(Name = "Label")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _label = null;

        [DataMember(Name = "ShapeType")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _shapeType = null;

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

        [DataMember(Name = "TextFont")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textFont = null;

        [DataMember(Name = "TextColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textColor = null;

        [DataMember(Name = "TextAlignH")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textAlignH = null;

        [DataMember(Name = "TextAlignV")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textAlignV = null;

        [DataMember(Name = "TextGrow")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textGrow = null;

        [DataMember(Name = "TextMargin")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _textMargin = -1;

        [DataMember(Name = "MinWidth")]
        [IgnoreIfDefaultValue(Default = 150)]
        private int _minWidth = 150;

        [DataMember(Name = "MinHeight")]
        [IgnoreIfDefaultValue(Default = 75)]
        private int _minHeight = 75;

        [DataMember(Name = "FillColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _fillColor = null;

        [DataMember(Name = "LineThick")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _lineThick = -1.0;

        [DataMember(Name = "LineColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _lineColor = null;

        [DataMember(Name = "LineLabel")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _lineLabel = null;

        [DataMember(Name = "Hide")]
        [IgnoreIfDefaultValue(Default = false)]
        private bool _hide = false;

        [DataMember(Name = "Truncate")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _truncate = -1;

        [DataMember(Name = "Note")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _note = null;

        [DataMember(Name = "ShapeConnectorType")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _shapeConnectorType = null;

        [DataMember(Name = "ShapeConnector")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<ShapeConnector> _shapeConnector = null;

        [DataMember(Name = "Hyperlink")]
        [IgnoreIfDefaultValue(Default = null)]
        private Hyperlink _hyperlink = null;

        [DataMember(Name = "TextHyperlink")]
        [IgnoreIfDefaultValue(Default = null)]
        private Hyperlink _textHyperlink = null;

        [DataMember(Name = "Table")]
        [IgnoreIfDefaultValue(Default = null)]
        private Table _table = null;

        [DataMember(Name = "Image")]
        [IgnoreIfDefaultValue(Default = null)]
        private Image _imageUrl = null;

        [DataMember(Name = "ShapeContainer")]
        [IgnoreIfDefaultValue(Default = null)]
        private ShapeContainer _shapeContainer = null;
        
        /// <summary>
        /// The ID of this shape. IDs are used to specify the paths of Return lines. IDs are arbitrary but should be unique.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The text label inside the shape.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// The type of shape. Used to change the shape from a rectangle (default) to another shape. A value from the ShapeTypes enum.
        /// </summary>
        public string ShapeType
        {
            get { return _shapeType; }
            set { _shapeType = value; }
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
        /// Sets the space (in 600 dpi) between the text and the outside edge of the shape.
        /// </summary>
        public int TextMargin
        {
            get { return _textMargin; }
            set { _textMargin = value; }
        }

        /// <summary>
        /// The color of the text label of the shape (hex RGB value). If omitted, the color is the default for the template.
        /// </summary>
        public string TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// The fill color of the shape (hex RGB value). If omitted, color is the default for the template.
        /// </summary>
        public string FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        /// <summary>
        /// Aligns the text label to the left, right or centered in the shape. If omitted, the alignment is the default for the template. A value from the HorizontalAlignments enum.
        /// </summary>
        public string TextAlignH
        {
            get { return _textAlignH; }
            set { _textAlignH = value; }
        }

        /// <summary>
        /// Align the text label to the top, bottom or middle in the shape. If omitted, the alignment is the default for the template. A value from the VerticalAlignments enum.
        /// </summary>
        public string TextAlignV
        {
            get { return _textAlignV; }
            set { _textAlignV = value; }
        }

        /// <summary>
        /// Determines the direction in which text will grow a shape. Values must be from the TextGrow enum.
        /// </summary>
        public string TextGrow
        {
            get { return _textGrow; }
            set { _textGrow = value; }
        }

        /// <summary>
        /// Specifies the initial width of a shape in 1/100" before any text is added. Adding more text than will fit into the shape will grow it according to the TextGrow value. The default value for MinWidth is 150.
        /// </summary>
        public int MinWidth
        {
            get { return _minWidth; }
            set { _minWidth = value; }
        }

        /// <summary>
        ///Specifies the initial height of a shape in 1/100" before any text is added. Adding more text than will fit into the shape will grow it according to the TextGrow value. The default value for MinHeight is 75.
        /// </summary>
        public int MinHeight
        {
            get { return _minHeight; }
            set { _minHeight = value; }
        }

        /// <summary>
        /// The border thickness of the shape in 1/100”. If omitted, the thickness is the default for the template.
        /// /// </summary>
        public double LineThick
        {
            get { return _lineThick; }
            set { _lineThick = value; }
        }

        /// <summary>
        /// The border color of the shape (hex RGB value). If omitted, color is the default for the template.
        /// </summary>
        public string LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// A text label on the connector line segment that touches the shape. Only Applies to Shape
        /// </summary>
        public string LineLabel
        {
            get { return _lineLabel; }
            set { _lineLabel = value; }
        }

        /// <summary>
        /// Whether or not to display the parent shape of a ShapeArray.
        /// </summary>
        public bool Hide
        {
            get { return _hide; }
            set { _hide = value; }
        }

        /// <summary>
        /// Defines the number of characters to allow in a shape or cell before the remaining text is truncated. By default nothing is truncated. Defining Truncate to “-1” turns it off if on by default.
        /// </summary>
        public int Truncate
        {
            get { return _truncate; }
            set { _truncate = value; }
        }

        /// <summary>
        /// Defines the type of connector that is coming off of this shape. Must be a value from ShapeConnectorTypes.
        /// </summary>
        public string ShapeConnectorType
        {
            get { return _shapeConnectorType; }
            set { _shapeConnectorType = value; }
        }

        /// <summary>
        /// A string to appear as a note attached to a shape.
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        /// <summary>
        /// The list of ShapeConnectors that are attached to and branch off from this shape.
        /// </summary>
        public List<ShapeConnector> ShapeConnector
        {
            get { return _shapeConnector; }
            set { _shapeConnector = value; }
        }

        /// <summary>
        /// A hyperlink on the shape.
        /// </summary>
        public Hyperlink Hyperlink
        {
            get { return _hyperlink; }
            set { _hyperlink = value; }
        }

        /// <summary>
        /// A hyperlink on the text in the shape.
        /// </summary>
        public Hyperlink TextHyperlink
        {
            get { return _textHyperlink; }
            set { _textHyperlink = value; }
        }

        /// <summary>
        /// Divides the shape up into rows and columns. 
        /// </summary>
        public Table Table
        {
            get { return _table; }
            set { _table = value; }
        }

        /// <summary>
        /// An image to put into the shape.
        /// </summary>
        public Image ImageURL
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        /// <summary>
        /// Defines arrangements of shapes not connected by lines, but contained inside a parent shape in rows, columns or a matrix arrangement
        /// </summary>
        public ShapeContainer ShapeContainer
        {
            get { return _shapeContainer; }
            set { _shapeContainer = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Shape()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Shape(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override void Serialize(List<System.Reflection.MemberInfo> members, SerializationInfo info, ref StreamingContext context, ref List<string> usedPropertyNames)
        {
            base.Serialize(members, info, ref context, ref usedPropertyNames);
        }
    }
}

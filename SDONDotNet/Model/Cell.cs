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
    /// Represents a Cell in a table.
    /// </summary>
    [Serializable]
    public sealed class Cell : SDONSerializeable
    {        
        [DataMember(Name = "Column")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _column = -1;

        [DataMember(Name = "Row")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _row = -1;

        [DataMember(Name = "Label")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _label = null;

        [DataMember(Name = "TextSize")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _textSize = -1.0;

        [DataMember(Name = "TextBold")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _textBold = null;

        [DataMember(Name = "TextItalic")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _textItalic = null;

        [DataMember(Name = "TextUnderline")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _textUnderline = null;

        [DataMember(Name = "TextFont")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textFont = null;

        [DataMember(Name = "TextColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textColor = null;

        [DataMember(Name = "FillColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _fillColor = null;

        [DataMember(Name = "TextAlignH")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textAlignH = null;

        [DataMember(Name = "TextAlignV")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textAlignV = null;

        [DataMember(Name = "Truncate")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _truncate = -1;

        [DataMember(Name = "Note")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _note = null;

        [DataMember(Name = "Hyperlink")]
        [IgnoreIfDefaultValue(Default = null)]
        private Hyperlink _hyperlink = null;

        [DataMember(Name = "TextHyperlink")]
        [IgnoreIfDefaultValue(Default = null)]
        private Hyperlink _textHyperlink = null;

        [DataMember(Name = "ImageURL")]
        [IgnoreIfDefaultValue(Default = null)]
        private Image _imageUrl = null;

        /// <summary>
        /// The column of the cell. Note that the first column is column 1 not column 0.
        /// </summary>
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        /// <summary>
        /// The row of the cell. Note that the first row is row 1 not row 0.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        /// <summary>
        /// The text label inside the cell.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
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
        /// The font of the text label the specified value. If omitted, the font is the default for the template. Any font can be defined, but will fall back to system default if font is unavailable
        /// </summary>
        public string TextFont
        {
            get { return _textFont; }
            set { _textFont = value; }
        }

        /// <summary>
        /// The color of the text label of the cell (hex RGB value). If omitted, the color is the default for the template.
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
        /// The number of characters to truncate text after.
        /// </summary>
        public int Truncate
        {
            get { return _truncate; }
            set { _truncate = value; }
        }

        /// <summary>
        /// A string to appear as a note attached to a cell.
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        /// <summary>
        /// A hyperlink in the cell.
        /// </summary>
        public Hyperlink Hyperlink
        {
            get { return _hyperlink; }
            set { _hyperlink = value; }
        }

        /// <summary>
        /// A hyperlink for the text in the cell.
        /// </summary>
        public Hyperlink TextHyperlink
        {
            get { return _textHyperlink; }
            set { _textHyperlink = value; }
        }

        /// <summary>
        /// Defines the url to the image to be shown in the cell.
        /// </summary>
        public Image ImageURL
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Cell()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Cell(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

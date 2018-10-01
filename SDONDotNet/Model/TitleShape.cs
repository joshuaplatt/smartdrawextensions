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
    /// Object that represents the title of a diagram.
    /// </summary>
    [Serializable]
    public sealed class TitleShape : SDONSerializeable
    {
        [DataMember(Name = "Label")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _label = null;

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
        [IgnoreIfDefaultValue(Default = null)]
        private int _textSize = -1;

        [DataMember(Name = "TextFont")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textFont = null;

        [DataMember(Name = "TextColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _textColor = null;

        /// <summary>
        /// The text to put as a title for the diagram.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// Makes the text label be bold with a value of true, not bold with false, otherwise, if omitted, boldness follows the template default.
        /// </summary>
        public bool TextBold
        {
            get { return (_textBold.HasValue == true) ? _textBold.Value : false; }
            set { _textBold = value; }
        }

        /// <summary>
        /// Makes the text label be italic with a value of true, not italic with false, otherwise, if omitted, italic follows the template default.
        /// </summary>
        public bool TextItalic
        {
            get { return (_textItalic.HasValue == true) ? _textItalic.Value : false; }
            set { _textItalic = value; }
        }

        /// <summary>
        /// Makes the text label be underlined with a value of true, not underlined with false, otherwise, if omitted, underlined follows the template default.
        /// </summary>
        public bool TextUnderline
        {
            get { return (_textUnderline.HasValue == true) ? _textUnderline.Value : false; }
            set { _textUnderline = value; }
        }

        /// <summary>
        /// The point size of the text label the specified value. If omitted, the text size is the default for the template.
        /// </summary>
        public int TextSize
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
        /// The color of the text label of the shape (hex RGB value). If omitted, the color is the default for the template.
        /// </summary>
        public string TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TitleShape()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal TitleShape(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

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
    /// Object that holds two color entries to alternate between while in a table.
    /// </summary>
    [Serializable]
    public sealed class TableAlternateRowsColors : SDONSerializeable
    {
        [DataMember(Name = "Color1")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _color1 = null;

        [DataMember(Name = "Color2")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _color2 = null;

        /// <summary>
        /// The first color to alternate between. If omitted, the default theme colors are used. Can be a hex color or a color alias.
        /// </summary>
        public string Color1
        {
            get { return _color1; }
            set { _color1 = value; }
        }

        /// <summary>
        /// The last color to alternate between. If omitted, the default theme colors are used. Can be a hex color or a color alias.
        /// </summary>
        public string Color2
        {
            get { return _color2; }
            set { _color2 = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TableAlternateRowsColors()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal TableAlternateRowsColors(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

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
    /// Object that maps an alias to a hex color code.
    /// </summary>
    [Serializable]
    public sealed class ColorEntry : SDONSerializeable
    {
        [DataMember(Name = "Name")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _name = null;

        [DataMember(Name = "Value")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _value = null;

        /// <summary>
        /// The alias to give the color.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The 6 or 8 character hex color, starting with the #. For example: #FFFFFF is the valid entry for white. The hex color code can be RGB or (8-digit) RGBA where A is the opacity of the color.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorEntry()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal ColorEntry(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

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
    /// Object for holding the reference to an image,
    /// </summary>
    [Serializable]
    public sealed class Image : SDONSerializeable
    {
        [DataMember(Name = "url")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _url = null;

        /// <summary>
        /// The URL of the image.
        /// </summary>
        public string url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Image()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Image(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

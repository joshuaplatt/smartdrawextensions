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
    /// Represents a container for a hyperlink.
    /// </summary>
    [Serializable]
    public sealed class Hyperlink : SDONSerializeable
    {
        [DataMember(Name = "url")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _url = null;

        /// <summary>
        /// The URL of the hyperlink.
        /// </summary>
        public string url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Hyperlink()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Hyperlink(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

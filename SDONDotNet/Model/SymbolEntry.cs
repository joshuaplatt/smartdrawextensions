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
    /// An entry that maps an alias to a symbol's GUID.
    /// </summary>
    [Serializable]
    public sealed class SymbolEntry : SDONSerializeable
    {
        [DataMember(Name = "Name")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _name = null;

        [DataMember(Name = "ID")]
        private Guid _value = Guid.Empty;

        /// <summary>
        /// The alias to give the symbol.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The ID of the symbol to use.
        /// </summary>
        public Guid ID
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SymbolEntry()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal SymbolEntry(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

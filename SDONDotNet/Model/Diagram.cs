using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SDON.Serialization;

namespace SDON.Model
{
    /// <summary>
    /// The root object of a SDON document page.
    /// </summary>
    [Serializable]
    public sealed class Diagram : SDONSerializeable
    {
        [DataMember(Name = "Template")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _template = null;

        [DataMember(Name = "Version")]
        private string _version = "20";

        [DataMember(Name = "Shape")]
        [IgnoreIfDefaultValue(Default = null)]
        private Shape _shape = null;

        [DataMember(Name = "Title")]
        [IgnoreIfDefaultValue(Default = null)]
        private TitleShape _title = null;

        [DataMember(Name = "Returns")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<Return> _returns = null;

        [DataMember(Name = "Colors")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<ColorEntry> _colors = null;

        [DataMember(Name = "Symbols")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<SymbolEntry> _symbols = null;

        [DataMember(Name = "DataTable")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<DataTable> _dataTable = null;

        [DataMember(Name = "UseDataTable")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<DataTableEntry> _useDataTable = null;

        /// <summary>
        /// Type of diagram SDON will create. Sets the behavior of lines and shapes. A value from the SDONTemplates enum.
        /// </summary>
        public string Template
        {
            get { return _template; }
            set { _template = value; }
        }

        /// <summary>
        /// The version of the SDON document.
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// A list of shapes indicating the root shapes (shapes starting without being connected to another shape) for the diagram.
        /// </summary>
        public Shape Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

        /// <summary>
        /// A title string centered over the diagram ½” above it.
        /// </summary>
        public TitleShape Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// A list of segmented lines that link shapes together.
        /// </summary>
        public List<Return> Returns
        {
            get { return _returns; }
            set {_returns = value;}
        }

        public List<ColorEntry> Colors
        {
            get { return _colors; }
            set { _colors = value; }
        }

        public List<SymbolEntry> Symbols
        {
            get { return _symbols; }
            set { _symbols = value; }
        }

        public List<DataTable> DataTable
        {
            get { return _dataTable; }
            set { _dataTable = value; }
        }

        public List<DataTableEntry> UseDataTable
        {
            get { return _useDataTable; }
            set { _useDataTable = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Diagram()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal Diagram(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override void Serialize(List<MemberInfo> members, SerializationInfo info, ref StreamingContext context, ref List<string> usedPropertyNames)
        {
            base.Serialize(members, info, ref context, ref usedPropertyNames);
        }
    }
}

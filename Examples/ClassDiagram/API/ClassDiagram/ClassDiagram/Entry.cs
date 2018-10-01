using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagram
{
    class Entry
    {
        public string TagName { get; set; }
        public string FileName { get; set; }
        public string Regex { get; set; }
        public string Kind { get; set; }
        public string Signature { get; set; }
        public string Access { get; set; }
        public string Namespace { get; set; }
        public string Class { get; set; }
        public string Inherits { get; set; }
        public string Typeref { get; set; }
        public string Language { get; set; }
        public string Line { get; set; }
        public string Separator { get; set; }
        
        public int HasAdjacentClone { get; set; }
        public bool GenericContext { get; set; }

        public int EntryID { get; set; }
        public int ParentID { get; set; }

        public Entry() { }

        public Entry(Entry copy)
        {
            TagName = copy.TagName;
            FileName = copy.FileName;
            Regex = copy.Regex;
            Kind = copy.Kind;
            this.Signature = copy.Signature;
            Access = copy.Access;
            Namespace = copy.Namespace;
            Class = copy.Class;
            Inherits = copy.Inherits;
            Typeref = copy.Typeref;
            Language = copy.Language;
            Line = copy.Line;
            Separator = copy.Separator;

            HasAdjacentClone = copy.HasAdjacentClone;
            GenericContext = copy.GenericContext;

            EntryID = copy.EntryID;
            ParentID = copy.ParentID;
        }
    }
}

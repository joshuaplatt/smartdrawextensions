using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagram
{
    class NamespaceContainer : Entry
    {
        public List<Entry> Members { get; set; }

        public NamespaceContainer(Entry parent)
            : base(parent)
        {
            Members = new List<Entry>();
        }
    }
}

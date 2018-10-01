using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagram
{
    class ClassContainer : Entry
    {
        public List<Entry> Members { get; set; }

        public ClassContainer(Entry parent)
            : base(parent)
        {
            Members = new List<Entry>();
        }
    }
}

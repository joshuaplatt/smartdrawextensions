using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagram
{
    class EntryShape
    {
        public SDON.Model.Shape Shape { get; set; }
        public ClassHierarchySection Hierarchy { get; set; }
        public Entry Entry { get; set; }
        public int ContainerIndex { get; set; }

        public EntryShape()
        {
            Hierarchy = new ClassHierarchySection();
        }
    }
}

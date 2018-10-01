using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagram
{
    class ClassHierarchySection
    {
        public SDON.Model.Shape ShapeColumn { get; set; }
        public SDON.Model.Shape ChildRow { get; set; }

        public ClassHierarchySection()
        {
            ShapeColumn = null;
            ChildRow = null;
        }
    }
}

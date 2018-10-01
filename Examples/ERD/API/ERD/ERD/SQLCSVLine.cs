using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD
{
    class SQLCSVLine
    {
        public string DatabaseName { get { return getLine(0); } }
        public string ParentSchema { get { return getLine(1); } }
        public string ParentTable { get { return getLine(2); } }
        public string ColumnName { get { return getLine(3); } }
        public string ColumnOrder { get { return getLine(4); } }
        public string DataType { get { return getLine(5); } }
        public string ColumnSize { get { return getLine(6); } }
        public string ConstraintType { get { return getLine(7); } }
        public string ChildSchema { get { return getLine(8); } }
        public string ChildTable { get { return getLine(9); } }
        public string ChildColumn { get { return getLine(10); } }

        private List<string> _line;

        private string getLine(int index)
        {
            try
            {
                return _line[index];
            }
            catch(ArgumentOutOfRangeException)
            {
                throw new Exception("Invalid CSV format.");
            }
        }

        public SQLCSVLine(List<string> line)
        {
            _line = line;
        }
    }
}

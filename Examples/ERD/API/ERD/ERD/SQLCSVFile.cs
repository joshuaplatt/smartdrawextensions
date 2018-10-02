using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD
{
    class SQLCSVFile
    {
        private CSVParser _converter;

        public SQLCSVLine GetLine(int line)
        {
            return new SQLCSVLine(_converter.Lines[line]);
        }

        public List<SQLCSVLine> ExportLines()
        {
            List<SQLCSVLine> ret = new List<SQLCSVLine>();

            for (int i = 0; i < _converter.Lines.Count; i++)
            {
                ret.Add(new SQLCSVLine(_converter.Lines[i]));
            }

            return ret;
        }

        public SQLCSVFile(string csv, bool isFile, char delimiter = ',')
        {
            _converter = new CSVParser();

            if(delimiter != ',')
            {
                _converter.ItemDelimiter = delimiter;
            }

            if (isFile)
            {
                _converter.ParseCSVFile(csv);
            }
            else
            {
                _converter.ParseCSVString(csv);
            }
        }
    }
}

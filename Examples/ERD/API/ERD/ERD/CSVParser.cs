using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD
{
    class CSVParser
    {
        public List<List<string>> Lines { get; private set; }

        private bool _mayBeDoubleQuote;
        private bool _isInQuotes;
        private Queue<byte> _value;
        private Queue<string> _csv;

        public char ItemDelimiter = ',';

        public void ParseCSVFile(string file)
        {
            System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open);
            byte[] buff = new byte[fs.Length];
            fs.Seek(0, System.IO.SeekOrigin.Begin);
            fs.Read(buff, 0, buff.Length);
            fs.Close();

            parseBuff(buff);
        }

        public void ParseCSVString(string csv)
        {
            parseBuff(UTF8Encoding.ASCII.GetBytes(csv));
        }

        public int GetLineCount()
        {
            return Lines.Count;
        }

        public int GetLineElementCount(int line)
        {
            return Lines[line].Count;
        }

        public string GetLineElement(int line, int element)
        {
            return Lines[line][element];
        }

        private void parseBuff(byte[] buff)
        {
            _isInQuotes = false;
            _mayBeDoubleQuote = false;

            _value = new Queue<byte>();
            _csv = new Queue<string>();
            Lines = new List<List<string>>();

            for (int i = 0; i < buff.Length; i++)
            {
                if (!_isInQuotes)
                {
                    parseCharOutsideQuotes(buff[i], buff, ref i);
                }
                else
                {
                    parseCharInsideQuotes(buff[i]);
                }
            }

            if (_value.Count > 0)
            {
                _csv.Enqueue(Encoding.UTF8.GetString(_value.ToArray()));
                _value = null;
            }

            if (_csv.Count > 0)
            {
                Lines.Add(new List<string>(_csv));
                _csv = null;
            }
        }

        private void parseCharOutsideQuotes(byte character, byte[] buff, ref int index)
        {
            if (character == '\r')
            {
                if ((_value.Count > 0) || ((index > 0) && (buff[index - 1] == ItemDelimiter)))
                {
                    _csv.Enqueue(Encoding.UTF8.GetString(_value.ToArray()));
                    _value.Clear();
                }

                if (_csv.Count > 0)
                {
                    Lines.Add(new List<string>(_csv));
                    _csv.Clear();
                }

                if (((index + 1) < buff.Length) && (buff[index + 1] == '\n'))
                {
                    index++;
                }
            }
            else if (character == '\n')
            {
                if ((_value.Count > 0) || ((index > 0) && (buff[index - 1] == ItemDelimiter)))
                {
                    _csv.Enqueue(Encoding.UTF8.GetString(_value.ToArray()));
                    _value.Clear();
                }

                if (_csv.Count > 0)
                {
                    Lines.Add(new List<string>(_csv));
                    _csv.Clear();
                }
            }
            else if (character == ItemDelimiter)
            {
                _csv.Enqueue(Encoding.UTF8.GetString(_value.ToArray()));
                _value.Clear();
            }
            else if (character == '"')
            {
                if (_mayBeDoubleQuote)
                {
                    _value.Enqueue((byte)'"');
                }

                _isInQuotes = true;
            }
            else
            {
                _value.Enqueue(character);
            }

            _mayBeDoubleQuote = false;
        }

        private void parseCharInsideQuotes(byte character)
        {
            if (character == '"')
            {
                _isInQuotes = false;
                _mayBeDoubleQuote = true;
            }
            else
            {
                _value.Enqueue(character);
            }
        }

        public CSVParser()
        {

        }
    }
}

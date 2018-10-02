using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD
{
    class SQLTable
    {
        private List<SQLCSVLine> _entries;
        private string _tableName;

        private bool _showColumns;
        private bool _showTypes;

        public int? ID { get; set; }

        public void AddEntry(SQLCSVLine entry)
        {
            _entries.Add(entry);
        }

        public int GetEntryCount()
        {
            return _entries.Count;
        }

        public SQLCSVLine GetEntry(int index)
        {
            return _entries[index];
        }

        public SDON.Model.Shape ExportAsShape()
        {
            const bool textBold = true;
            const int truncate = 20;
            const bool hasKeys = true;
            SDON.Model.Shape shape = new SDON.Model.Shape();
            shape.TextGrow = SDON.Model.TextGrow.Horizontal;
            shape.LineColor = "#BFBFBF";

            if (ID != null)
            {
                shape.ID = (int)ID;
            }

            if (!_showColumns)
            {
                shape.FillColor = "#D8D8D8";
                shape.Label = _tableName;
                shape.TextMargin = 6;
                shape.TextSize = 12;
                shape.NoteIcon = SDON.Model.Icons.Info;

                //SDON.Model.ColumnProperties cp = new SDON.Model.ColumnProperties();
                //cell = new SDON.Model.Cell();
                //
                //cell.Column = 2;
                //cell.Row = 1;
                //cell.Note = "";
                //cell.NoteIcon = SDON.Model.Icons.Info;
                //cell.FillColor = "#D8D8D8";
                //
                //cp.Index = 2;
                //cp.Width = 20;
                //cp.FixedWidth = true;

                for (int i = 0; i < _entries.Count; i++)
                {
                    shape.Note += _entries[i].ColumnName + "\n";
                }

                //shape.Table.Columns = 2;
                //shape.Table.Rows = 1;
                //shape.Table.Cell.Add(cell);
                //shape.Table.ColumnProperties = new List<SDON.Model.ColumnProperties>();
                //shape.Table.ColumnProperties.Add(cp);

                return shape;
            }

            shape.Table = new SDON.Model.Table();
            shape.Table.Cell = new List<SDON.Model.Cell>();
            shape.TextMargin = 6;
            SDON.Model.Cell cell;

            shape.Table.Columns = 1;
            shape.Table.Rows = _entries.Count + 1;
            cell = new SDON.Model.Cell();
            cell.Label = _tableName;
            cell.Column = 1;
            cell.Row = 1;
            cell.TextBold = textBold;
            //cell.Truncate = truncate;
            cell.FillColor = "#D8D8D8";
            cell.TextSize = 12;
            shape.Table.Cell.Add(cell);

            SDON.Model.RowProperties rprop; // = new SDON.Model.RowProperties();
            shape.Table.RowProperties = new List<SDON.Model.RowProperties>();
            //rprop.Index = 1;
            //rprop.LineThick = 2;
            //shape.Table.RowProperties.Add(rprop);

            SDON.Model.Join join = null;

            if (_showTypes)
            {
                shape.Table.Columns = 3;

                join = new SDON.Model.Join();
                join.Row = 1;
                join.Column = 1;
                join.N = 2;
                shape.Table.Join = new List<SDON.Model.Join>();
                shape.Table.Join.Add(join);
            }

            if (hasKeys)
            {
                shape.Table.Columns++;

                SDON.Model.ColumnProperties keyColumnProperties = new SDON.Model.ColumnProperties();
                keyColumnProperties.Width = 20;
                keyColumnProperties.FixedWidth = true;
                keyColumnProperties.Index = 1;

                if (shape.Table.ColumnProperties == null)
                {
                    shape.Table.ColumnProperties = new List<SDON.Model.ColumnProperties>();
                }

                shape.Table.ColumnProperties.Add(keyColumnProperties);

                if (join == null)
                {
                    join = new SDON.Model.Join();
                    join.Row = 1;
                    join.Column = 1;
                    shape.Table.Join = new List<SDON.Model.Join>();
                    shape.Table.Join.Add(join);
                    join.N = 0;
                }

                join.N++;
            }

            for (int i = 0; i < _entries.Count; i++)
            {
                if (_showTypes)
                {
                    createRowWithEntry(shape.Table, i + 2, _entries[i], true, hasKeys, true, true);
                }
                else
                {
                    createRowWithEntry(shape.Table, i + 2, _entries[i], true, hasKeys, false, false);
                }

                //cell = new SDON.Model.Cell();
                //cell.Label = _entries[i].ColumnName;
                //cell.Row = i + 2;
                //cell.Column = 1;
                //cell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                //cell.Truncate = 20;
                //cell.FillColor = "#FFFFFF";
                //shape.Table.Cell.Add(cell);
                //
                //if(_showTypes)
                //{
                //    //Types
                //    cell = new SDON.Model.Cell();
                //    cell.Label = _entries[i].DataType;
                //    cell.Row = i + 2;
                //    cell.Column = 2;
                //    cell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                //    cell.Truncate = 20;
                //    cell.FillColor = "#FFFFFF";
                //    shape.Table.Cell.Add(cell);
                //
                //    //Size
                //    cell = new SDON.Model.Cell();
                //    cell.Label = (_entries[i].ColumnSize != "NULL") ? _entries[i].ColumnSize : "";
                //    cell.Row = i + 2;
                //    cell.Column = 3;
                //    cell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                //    cell.Truncate = 20;
                //    cell.FillColor = "#FFFFFF";
                //    shape.Table.Cell.Add(cell);
                //}

                rprop = new SDON.Model.RowProperties();
                rprop.LineColor = "#D8D8D8";
                rprop.Index = i + 1;
            }

            return shape;
        }

        private void createRowWithEntry(SDON.Model.Table table, int rowNum, SQLCSVLine entry, bool showLabel, bool showKey, bool showType, bool showSize)
        {
            SDON.Model.Cell cell;
            int column = 1;

            if (showKey)
            {
                bool isForeignKey = (entry.ConstraintType.ToLower() == "foreign key") || (entry.ConstraintType.ToLower() == "r");
                bool isPrimaryKey = (entry.ConstraintType.ToLower() == "primary key") || (entry.ConstraintType.ToLower() == "p");
                bool isUniqueKey = (entry.ConstraintType.ToLower() == "unique") || (entry.ConstraintType.ToLower() == "u");

                cell = createCellTemplate();
                cell.Row = rowNum;
                cell.Column = column;
                //cell.ImageURL = new SDON.Model.Image();

                if (isForeignKey)
                {
                    //cell.ImageURL.url = "";
                    cell.Label = "F";
                }
                else if (isPrimaryKey)
                {
                    //cell.ImageURL.url = "";
                    cell.Label = "P";
                }
                else if (isUniqueKey)
                {
                    //cell.ImageURL.url = "";
                    cell.Label = "U";
                }
                else
                {
                    //cell.ImageURL = null;
                }

                table.Cell.Add(cell);
                column++;
            }

            if (showLabel)
            {
                cell = createCellTemplate();
                cell.Row = rowNum;
                cell.Column = column;
                cell.Label = entry.ColumnName;
                table.Cell.Add(cell);
                column++;
            }

            if (showType)
            {
                cell = createCellTemplate();
                cell.Row = rowNum;
                cell.Column = column;
                cell.Label = entry.DataType;
                table.Cell.Add(cell);
                column++;
            }

            if (showSize)
            {
                cell = createCellTemplate();
                cell.Row = rowNum;
                cell.Column = column;
                cell.Label = (entry.ColumnSize != "NULL") ? entry.ColumnSize : "";
                table.Cell.Add(cell);
                column++;
            }
        }

        private SDON.Model.Cell createCellTemplate()
        {
            SDON.Model.Cell ret = new SDON.Model.Cell();
            ret.FillColor = "#FFFFFF";
            //ret.Truncate = 20;
            ret.TextAlignH = SDON.Model.HorizontalAlignments.Left;

            return ret;
        }

        public string GetTableName()
        {
            return _tableName;
        }

        public SQLTable(string name, bool showColumns, bool showTypes)
        {
            _entries = new List<SQLCSVLine>();
            _tableName = name;
            _showColumns = showColumns;
            _showTypes = showTypes;
        }
    }
}

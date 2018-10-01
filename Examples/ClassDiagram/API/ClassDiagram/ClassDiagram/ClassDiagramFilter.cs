﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * c  classes
 * d  macro definitions
 * e  enumerators (values inside an enumeration)
 * E  events
 * f  fields
 * g  enumeration names
 * i  interfaces
 * l  local variables [off]
 * m  methods
 * n  namespaces
 * p  properties
 * s  structure names
 * t  typedefs
 */

namespace ClassDiagram
{
    public class ClassDiagramFilter
    {
        /// <summary>
        /// Create a function matching this delegate to have custom link
        /// generation to a web page relative to directory specified by the
        /// CTAGS file and line number.
        /// </summary>
        /// <param name="fileDirectory">File specified by CTAGS file, will be
        /// relative to the folder that CTAGS was run from.</param>
        /// <param name="line">Line number within the file that this token was
        /// declared.</param>
        /// <param name="userdata">Anonymous object to contain custom data to
        /// refer to during generation process, or <c>null</c> if no userdata
        /// has been specified.</param>
        /// <returns>The URL of the link to be generated, or <c>null</c> to not
        /// generate a URL for this entry.</returns>
        /// <seealso cref="LinkGenerator"/>
        public delegate string LinkGeneratorDelegate(string fileDirectory, int line, Object userdata);

        /// <summary>
        /// Set this to a function matching LinkGeneratorDelegate to generate
        /// hyperlinks.  If this is not specified, it will be ignored and no
        /// hyperlinks will be created.
        /// </summary>
        public LinkGeneratorDelegate LinkGenerator { get; set; }

        /// <summary>
        /// Anonymous object to contain custom data to refer to during diagram
        /// generation for use in the LinkGenerator.
        /// </summary>
        /// <seealso cref="LinkGeneratorDelegate"/>
        public Object Userdata { get; set; }

        /// <summary>
        /// Set to true to have methods be hidden and placed in a note within
        /// the table.
        /// </summary>
        public bool MethodsInNote { get; set; }

        /// <summary>
        /// Set to true to have properties be hidden and placed in a note
        /// within the table.
        /// </summary>
        public bool PropertiesInNote { get; set; }

        /// <summary>
        /// Set to true to have the signature to a method be placed in a note
        /// next to the method.  Has no effect if MethodsInNote is set to true.
        /// </summary>
        public bool SignatureInNote { get; set; }

        private SDON.Model.Diagram _root;
        private List<EntryShape> _nodesOnTree;

        private Entry[] _entries;
        private NamespaceContainer[] _namespaces;
        private ClassContainer[] _classes;

        private CtagsParser _parser;

        /// <summary>
        /// Loads a CTAGS output file to be parsed by the filter.  Will have
        /// been already generated by CTAGS.
        /// (Recommended: <a href="https://github.com/universal-ctags/ctags">https://github.com/universal-ctags/ctags</a>)
        /// Only one file can be loaded at once.
        /// </summary>
        /// <param name="file">Path to the CTAGS output file.</param>
        public void LoadFromCtagsFile(string file)
        {
            System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open);
            int length = (int)fs.Length;
            byte[] buff = new byte[length];
            fs.Seek(0, System.IO.SeekOrigin.Begin);

            fs.Read(buff, 0, length);
            fs.Close();

            string fileContents = Encoding.UTF8.GetString(buff);
            LoadFromCtagsString(fileContents);
        }

        /// <summary>
        /// Loads a string representation of a CTAGS output file.  Only one
        /// file can be loaded at once.
        /// </summary>
        /// <param name="ctags">The string representation of a CTAGS output file.</param>
        public void LoadFromCtagsString(string ctags)
        {
            _parser.ParseCtags(ctags);
            _entries = _parser.GetEntries();
            _classes = _parser.GetClasses();
            _namespaces = _parser.GetNamespaces();

            EntryShape temp;

            for (int i = 0; i < _entries.Length; i++)
            {
                temp = generateEntryShape(i + 1);
                if (temp != null)
                {
                    _nodesOnTree.Add(temp);
                }
            }
        }

        /// <summary>
        /// Converts the CTAGS file and generates the diagram.  Make sure that
        /// the CTAGS file was already loaded using one of the Load From Ctags
        /// methods.
        /// </summary>
        /// <returns>The generated diagram from the loaded CTAGS file.</returns>
        /// <seealso cref="LoadFromCtagsFile(string)"/>
        /// <seealso cref="LoadFromCtagsString(string)"/>
        public SDON.Model.Diagram ConvertCtags()
        {
            //_root.RootShape.Add(new SDON.Model.Shape());
            //_root.RootShape[0].Label = "Program";

            for (int i = 0; i < _nodesOnTree.Count; i++)
            {
                generateNodeToTree(i);
            }

            return _root;
        }

        /// <summary>
        /// Gets the root after the diagram is generated.  For use for internal
        /// diagram manipulation after generation.
        /// </summary>
        /// <returns>The generated diagram, or <c>null</c> if the CTAGS file
        /// has not been converted.</returns>
        /// <seealso cref="ConvertCtags"/>
        public SDON.Model.Diagram GetDiagram()
        {
            return _root;
        }

        /// <summary>
        /// Saves the diagram to an SDON file after the diagram has been
        /// generated.
        /// </summary>
        /// <param name="outputFile">Path for the file that will be generated.</param>
        /// <seealso cref="ConvertCtags"/>
        public void SaveConvertedDocument(string outputFile)
        {
            string jsonData = SDON.SDONBuilder.ToJSON(_root);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputFile, false, Encoding.UTF8))
            {
                file.Write(jsonData);
            }
        }

        private EntryShape generateEntryShape(int index)
        {
            if (_entries[index - 1].Kind == "namespace")
            {
                for (int i = 0; i < _namespaces.Length; i++)
                {
                    if (index == _namespaces[i].EntryID)
                    {
                        EntryShape shape = new EntryShape();
                        SDON.Model.Cell cell = new SDON.Model.Cell();
                        SDON.Model.Cell auxCell = new SDON.Model.Cell();

                        shape.Shape = generateShapeTemplate();
                        shape.Entry = _entries[index - 1];
                        shape.Shape.ID = _entries[index - 1].EntryID;
                        shape.ContainerIndex = i;

                        cell.Label = shape.Entry.TagName + "\nnamespace";
                        cell.Row = 1;
                        cell.Column = 1;
                        cell.TextSize = 12;
                        cell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                        cell.FillColor = "#B0CCE3";

                        auxCell.Row = 1;
                        auxCell.Column = 2;
                        auxCell.TextSize = 12;
                        auxCell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                        auxCell.FillColor = "#B0CCE3";

                        if (LinkGenerator != null)
                        {
                            auxCell.Hyperlink = new SDON.Model.Hyperlink();
                            auxCell.Hyperlink.url = LinkGenerator(_entries[index - 1].FileName, int.Parse(_entries[index - 1].Line), Userdata);
                        }

                        shape.Shape.Table.Cell.Add(cell);
                        shape.Shape.Table.Cell.Add(auxCell);

                        return shape;
                    }
                }
            }
            else if (_entries[index - 1].Kind == "class")
            {
                for (int i = 0; i < _classes.Length; i++)
                {
                    if (index == _classes[i].EntryID)
                    {
                        EntryShape shape = new EntryShape();
                        SDON.Model.Cell cell = new SDON.Model.Cell();
                        SDON.Model.Cell auxCell = new SDON.Model.Cell();

                        shape.Shape = generateShapeTemplate();
                        shape.Entry = _entries[index - 1];
                        shape.Shape.ID = _entries[index - 1].EntryID;
                        shape.ContainerIndex = i;

                        cell.Label = shape.Entry.TagName + "\nclass";
                        cell.Row = 1;
                        cell.Column = 1;
                        cell.TextSize = 12;
                        cell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                        cell.FillColor = "#B0CCE3";

                        auxCell.Row = 1;
                        auxCell.Column = 2;
                        auxCell.TextSize = 12;
                        auxCell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                        auxCell.FillColor = "#B0CCE3";

                        if (LinkGenerator != null)
                        {
                            auxCell.Hyperlink = new SDON.Model.Hyperlink();
                            auxCell.Hyperlink.url = LinkGenerator(_entries[index - 1].FileName, int.Parse(_entries[index - 1].Line), Userdata);
                        }

                        shape.Shape.Table.Cell.Add(cell);
                        shape.Shape.Table.Cell.Add(auxCell);

                        return shape;
                    }
                }
            }

            return null;
        }

        private SDON.Model.Shape generateShapeTemplate()
        {
            SDON.Model.Shape ret = new SDON.Model.Shape();
            ret.Table = new SDON.Model.Table();
            ret.Table.Cell = new List<SDON.Model.Cell>();
            //ret.Table.RowProperties = new List<SDON.Model.RowProperties>();
            ret.Table.ColumnProperties = new List<SDON.Model.ColumnProperties>();
            //SDON.Model.RowProperties rprop = new SDON.Model.RowProperties();
            SDON.Model.ColumnProperties cprop = new SDON.Model.ColumnProperties();
            SDON.Model.Join join = new SDON.Model.Join();
            //rprop.LineThick = 2;
            //rprop.Index = 1;
            cprop.Width = 40;
            cprop.FixedWidth = true;
            cprop.Index = 2;
            
            ret.Table.Columns = 2;
            ret.Table.Rows = 1;
            //ret.Table.ColumnWidth = 300;
            ret.ShapeType = "Rect";
            ret.FillColor = "#FFFFFF";
            ret.TextMargin = 6;
            ret.TextGrow = SDON.Model.TextGrow.Horizontal;
            //ret.TextFont = "Courier New";
            //ret.Table.RowProperties.Add(rprop);
            ret.Table.ColumnProperties.Add(cprop);

            join.Column = 1;
            join.Row = 1;
            join.N = 1;

            ret.Table.Join = new List<SDON.Model.Join>();
            //ret.Table.Join.Add(join);

            return ret;
        }

        private void generateNodeToTree(int index)
        {
            List<Entry> members;
            SDON.Model.Cell cell;
            SDON.Model.Cell auxCell;
            SDON.Model.Join join;
            SDON.Model.ShapeConnector connector;
            List<SDON.Model.Shape> parent = null;
            SDON.Model.Shape row;
            SDON.Model.Shape column;
            int i;
            bool methodsInit = false;
            bool propertiesInit = false;

            if (_nodesOnTree[index].Entry.Kind == "class")
            {
                members = _classes[_nodesOnTree[index].ContainerIndex].Members;
            }
            else if (_nodesOnTree[index].Entry.Kind == "namespace")
            {
                members = _namespaces[_nodesOnTree[index].ContainerIndex].Members;
            }
            else
            {
                return;
            }

            if(_nodesOnTree[index].Shape.Table.Cell.Last().Row != 1)
            {
                i = _nodesOnTree[index].Shape.Table.Cell.Count - 1;

                while((i > 0) && (_nodesOnTree[index].Shape.Table.Cell[i].Row != 1))
                {
                    i--;
                }

                _nodesOnTree[index].Shape.Table.Cell.RemoveRange(i, _nodesOnTree[index].Shape.Table.Cell.Count - i);
            }

            //Insert methods (if enabled)
            if (!MethodsInNote)
            {
                for (i = 0; i < members.Count; i++)
                {
                    if (((members[i].Kind == "method") || (members[i].Kind == "function")) && (members[i].Access != "private") &&
                        (members[i].Access != "protected"))
                    {
                        if (!methodsInit)
                        {
                            insertSectionToNode("Methods", _nodesOnTree[index].Shape.Table);
                            methodsInit = true;

                            join = new SDON.Model.Join();
                            join.Row = _nodesOnTree[index].Shape.Table.Rows;
                            join.Column = 1;
                            join.N = 1;

                            if (_nodesOnTree[index].Shape.Table.Join == null)
                            {
                                _nodesOnTree[index].Shape.Table.Join = new List<SDON.Model.Join>();
                            }

                            //_nodesOnTree[index].Shape.Table.Join.Add(join);
                        }

                        insertEntryToNode(members[i], _nodesOnTree[index].Shape.Table);
                    }
                }
            }
            else
            {
                cell = null;
                auxCell = null;

                for (i = 0; i < members.Count; i++) {
                    if (((members[i].Kind == "method") || (members[i].Kind == "function")) && (members[i].Access != "private") &&
                        (members[i].Access != "protected"))
                    {
                        if (!methodsInit)
                        {
                            insertSectionToNode("Methods", _nodesOnTree[index].Shape.Table);
                            methodsInit = true;

                            auxCell = _nodesOnTree[index].Shape.Table.Cell.Last();
                            auxCell.Note = "";
                        }

                        auxCell.Note += _entries[members[i].EntryID - 1].TagName + generateSignature(members[i].EntryID) + "\n";
                    }
                }
            }

            if (!PropertiesInNote)
            {
                for (i = 0; i < members.Count; i++)
                {
                    if (((members[i].Kind == "property") || (members[i].Kind == "variable")) && (members[i].Access != "private") &&
                        (members[i].Access != "protected"))
                    {
                        if (!propertiesInit)
                        {
                            insertSectionToNode("Properties", _nodesOnTree[index].Shape.Table);
                            propertiesInit = true;

                            join = new SDON.Model.Join();
                            join.Row = _nodesOnTree[index].Shape.Table.Rows;
                            join.Column = 1;
                            join.N = 1;

                            if (_nodesOnTree[index].Shape.Table.Join == null)
                            {
                                _nodesOnTree[index].Shape.Table.Join = new List<SDON.Model.Join>();
                            }

                            //_nodesOnTree[index].Shape.Table.Join.Add(join);
                        }

                        insertEntryToNode(members[i], _nodesOnTree[index].Shape.Table);
                    }
                }
            }
            else
            {
                cell = null;
                auxCell = null;
                
                for (i = 0; i < members.Count; i++) {
                    if (((members[i].Kind == "property") || (members[i].Kind == "variable")) && (members[i].Access != "private") &&
                        (members[i].Access != "protected"))
                    {
                        if (!propertiesInit)
                        {
                            insertSectionToNode("Properties", _nodesOnTree[index].Shape.Table);
                            propertiesInit = true;

                            auxCell = _nodesOnTree[index].Shape.Table.Cell.Last();
                            auxCell.Note = "";
                        }
                        
                        auxCell.Note += _entries[members[i].EntryID - 1].TagName + generateSignature(members[i].EntryID) + "\n";
                    }
                }
            }

            parent = findParent(_nodesOnTree[index].Entry.ParentID);

            if (parent != null)
            {
                parent.Add(_nodesOnTree[index].Shape);
            }
        }

        private void insertEntryToNode(Entry member, SDON.Model.Table table)
        {
            SDON.Model.Cell cell = new SDON.Model.Cell();
            SDON.Model.Cell auxCell = new SDON.Model.Cell();

            cell.Row = table.Rows + 1;
            cell.Column = 1;
            //cell.Label = generateEntryString(member.EntryID);
            cell.Label = _entries[member.EntryID - 1].TagName;
            cell.TextSize = 10;
            cell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
            cell.FillColor = "#FFFFFF";
            cell.Truncate = 42;

            auxCell.Row = table.Rows + 1;
            auxCell.Column = 2;
            auxCell.TextSize = 10;
            auxCell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
            auxCell.FillColor = "#FFFFFF";
            auxCell.NoteIcon = "Info";

            if (!SignatureInNote)
            {
                cell.Label += generateSignature(member.EntryID);
            }
            else
            {
                auxCell.Note = generateSignature(member.EntryID);
            }

            if (LinkGenerator != null)
            {
                auxCell.Hyperlink = new SDON.Model.Hyperlink();
                auxCell.Hyperlink.url = LinkGenerator(member.FileName, int.Parse(member.Line), Userdata);
            }

            table.Cell.Add(cell);
            table.Cell.Add(auxCell);

            table.Rows++;
        }

        private void insertSectionToNode(string sectionName, SDON.Model.Table table)
        {
            SDON.Model.Cell cell = new SDON.Model.Cell();
            SDON.Model.Cell auxCell = new SDON.Model.Cell();

            cell.Row = table.Rows + 1;
            cell.Column = 1;
            cell.Label = sectionName;
            cell.TextSize = 10;
            cell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
            cell.FillColor = "#EFF4F9";
            cell.TextUnderline = false;
            cell.Truncate = 42;

            auxCell.Row = table.Rows + 1;
            auxCell.Column = 2;
            auxCell.TextSize = 10;
            auxCell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
            auxCell.FillColor = "#EFF4F9";
            auxCell.NoteIcon = "Info";

            table.Cell.Add(cell);
            table.Cell.Add(auxCell);
            table.Rows++;
        }

        private List<SDON.Model.Shape> findParent(int parentID)
        {
            SDON.Model.ShapeConnector con;
            List<SDON.Model.Shape> parent = null;

            if (parentID == 0)
            {
                if (_root.Shape == null)
                {
                    //SHAPE ARRAY
                    _root.Shape = new SDON.Model.Shape();
                    _root.Shape.Hide = true;
                    _root.Shape.ShapeContainer = new SDON.Model.ShapeContainer();
                    _root.Shape.ShapeContainer.Arrangement = SDON.Model.ShapeArrangementTypes.Row;

                    parent = new List<SDON.Model.Shape>();

                    _root.Shape.ShapeContainer.Shapes = parent;
                }
                else
                {
                    parent = _root.Shape.ShapeContainer.Shapes;
                }
            }
            else
            {
                for (int i = 0; i < _nodesOnTree.Count; i++)
                {
                    if (_nodesOnTree[i].Entry.EntryID == parentID)
                    {
                        if (_nodesOnTree[i].Shape.ShapeConnector == null)
                        {
                            _nodesOnTree[i].Shape.ShapeConnector = new List<SDON.Model.ShapeConnector>();
                            parent = new List<SDON.Model.Shape>();

                            con = new SDON.Model.ShapeConnector();
                            con.Shapes = parent;
                            con.StartArrow = 1;
                            con.EndArrow = 0;
                            con.Arrangement = "Row";

                            _nodesOnTree[i].Shape.ShapeConnector.Add(con);
                        }
                        else
                        {
                            parent = _nodesOnTree[i].Shape.ShapeConnector[0].Shapes;
                        }

                        break;
                    }
                }
            }

            return parent;
        }

        private void insertReturn(int index, int startShapeIndex)
        {
            if (_entries[index - 1].Typeref == null)
            {
                return;
            }

            for (int i = 0; i < _nodesOnTree.Count; i++)
            {
                if ((_nodesOnTree[i].Entry.EntryID != startShapeIndex) && ((_nodesOnTree[i].Entry.TagName == _entries[index - 1].Typeref) ||
                    /*(false)))*/((_nodesOnTree[i].Entry.TagName + " *") == _entries[index - 1].Typeref))) //Clutters like crazy
                {
                    SDON.Model.Return ret = new SDON.Model.Return();
                    ret.StartID = startShapeIndex;
                    ret.EndID = _nodesOnTree[i].Entry.EntryID;
                    ret.LinePattern = SDON.Model.LinePatterns.Dashed;

                    if (!checkIfReturnExists(ret))
                    {
                        if(_root.Returns == null)
                        {
                            _root.Returns = new List<SDON.Model.Return>();
                        }

                        _root.Returns.Add(ret);
                    }

                    return;
                }
            }
        }

        private bool checkIfReturnExists(SDON.Model.Return ret)
        {
            if(_root.Returns == null)
            {
                return false;
            }

            for (int i = 0; i < _root.Returns.Count; i++)
            {
                if ((_root.Returns[i].EndID == ret.EndID) && (_root.Returns[i].StartID == ret.StartID))
                {
                    return true;
                }
                else if ((_root.Returns[i].EndID == ret.StartID) && (_root.Returns[i].StartID == ret.EndID))
                {
                    _root.Returns[i].StartArrow = 1;
                    return true;
                }
            }

            return false;
        }

        private string generateSignature(int index)
        {
            string ret = "";

            if (_entries[index - 1].Signature != null)
            {
                ret += _entries[index - 1].Signature;
            }
            if (_entries[index - 1].Typeref != null)
            {
                ret += " : " + _entries[index - 1].Typeref;
            }

            return ret;
        }

        public ClassDiagramFilter()
        {
            _root = new SDON.Model.Diagram();
            _root.Version = "20";
            _root.Template = "Classdiagram";

            _nodesOnTree = new List<EntryShape>();

            _parser = new CtagsParser();
            LinkGenerator = null;
            Userdata = null;
            MethodsInNote = false;
            PropertiesInNote = false;
            SignatureInNote = false;
        }
    }
}

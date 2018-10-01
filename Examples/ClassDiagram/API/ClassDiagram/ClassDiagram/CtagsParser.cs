using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagram
{
    class CtagsParser
    {
        private List<Entry> _entries;

        private ClassContainer[] _classes;
        private ClassContainer[] _classesByTag;
        private NamespaceContainer[] _namespaces;

        private EntrySorter<ClassContainer> _classSorter;
        private EntrySorter<ClassContainer> _classSorterByTag;
        private EntrySorter<NamespaceContainer> _namespaceSorter;

        private const string CSNSSeparator = ".";
        private const string CPPNSSeparator = "::";

        public void ParseCtags(string ctags)
        {
            string[] lines = ctags.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i][0] != '!')
                {
                    parseCtagsLine(lines[i]);
                }
            }

            insertContainers();
            insertEntriesToContainers();
        }

        public Entry[] GetEntries()
        {
            return _entries.ToArray();
        }

        public ClassContainer[] GetClasses()
        {
            return _classes;
        }

        public NamespaceContainer[] GetNamespaces()
        {
            return _namespaces;
        }

        private void parseCtagsLine(string line)
        {
            int exSplit = line.IndexOf(";\"");
            exSplit = (exSplit >= 0) ? exSplit : line.Length;
            string[] values = line.Substring(0, exSplit).Split(new char[] {'\t'}, 3);
            string[] exList = (line.Length > (exSplit + 2)) ? line.Substring(exSplit + 2).Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries) : null;
            Entry entry = new Entry();

            if (values.Length < 3)
            {
                throw new Exception("Invalid ctags format: Too few elements on line");
            }

            entry.TagName = values[0];
            entry.FileName = values[1];
            entry.Regex = values[2];

            if (exList != null)
            {
                parseCtagsEx(exList, entry);
            }

            entry.EntryID = _entries.Count + 1;
            _entries.Add(entry);
        }

        private void parseCtagsEx(string[] exList, Entry entry)
        {
            int endLoc;

            for (int i = 0; i < exList.Length; i++)
            {
                endLoc = exList[i].IndexOf(':');

                if (((endLoc + 1) < exList[i].Length) && (endLoc >= 0))
                {
                    switch (exList[i].Substring(0, endLoc))
                    {
                        case "kind":
                            entry.Kind = exList[i].Substring(endLoc + 1);
                            break;
                        case "namespace":
                            entry.Namespace = exList[i].Substring(endLoc + 1);
                            break;
                        case "class":
                            entry.Class = exList[i].Substring(endLoc + 1);
                            break;
                        case "signature":
                            entry.Signature = exList[i].Substring(endLoc + 1);
                            break;
                        case "access":
                            entry.Access = exList[i].Substring(endLoc + 1);
                            break;
                        case "inherits":
                            entry.Inherits = exList[i].Substring(endLoc + 1);
                            break;
                        case "typeref":
                            int newSearch = exList[i].IndexOf(':', endLoc + 1);

                            if (((newSearch + 1) < exList[i].Length) && (newSearch >= 0))
                            {
                                entry.Typeref = exList[i].Substring(newSearch + 1);
                            }
                            break;
                        case "language":
                            entry.Language = exList[i].Substring(endLoc + 1);

                            //Separator detection
                            if (entry.Language == "C#")
                            {
                                entry.Separator = CSNSSeparator;
                            }
                            else if (entry.Language == "C++")
                            {
                                entry.Separator = CPPNSSeparator;
                            }
                            else
                            {
                                entry.Separator = ".";  //default
                            }

                            break;
                        case "line":
                            entry.Line = exList[i].Substring(endLoc + 1);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void insertContainers()
        {
            List<ClassContainer> classes = new List<ClassContainer>();
            List<NamespaceContainer> namespaces = new List<NamespaceContainer>();

            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Kind == "class")
                {
                    classes.Add(new ClassContainer(_entries[i]));
                }
                else if (_entries[i].Kind == "namespace")
                {
                    namespaces.Add(new NamespaceContainer(_entries[i]));
                }
            }

            _classSorter = new EntrySorter<ClassContainer>(classes);
            _classSorter.SortByStrict();
            _classes = _classSorter.GetArray();

            _classSorterByTag = new EntrySorter<ClassContainer>(classes);
            _classSorterByTag.SortByTag();
            _classesByTag = _classSorterByTag.GetArray();

            _namespaceSorter = new EntrySorter<NamespaceContainer>(namespaces);
            _namespaceSorter.SortByStrict();
            _namespaces = _namespaceSorter.GetArray();
        }

        private void insertEntriesToContainers()
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                

                if (_entries[i].Inherits != null)
                {
                    insertEntryToInheritedClass(i);
                }
                else if (_entries[i].Class != null)
                {
                    insertEntryToClass(i);
                }
                else if (_entries[i].Namespace != null)
                {
                    insertEntryToNamespace(i);
                }
            }
        }

        private void insertEntryToInheritedClass(int index)
        {
            Entry temp = new Entry();
            temp.TagName = _entries[index].Inherits;

            ClassContainer cmp = new ClassContainer(temp);

            int i = Array.BinarySearch(_classesByTag, cmp, _classSorterByTag);

            if (i >= 0)
            {
                _entries[index].ParentID = _classesByTag[i].EntryID;
                _classesByTag[i].Members.Add(_entries[index]);
            }
            else
            {
                int x = 0;
            }
        }

        private void insertEntryToClass(int index)
        {
            Entry temp = new Entry();
            temp.TagName = _entries[index].Class;
            temp.GenericContext = true;
            temp.Separator = _entries[index].Separator;

            ClassContainer cmp = new ClassContainer(temp);

            int i = Array.BinarySearch(_classes, cmp, _classSorter);

            if (i >= 0)
            {
                _entries[index].ParentID = _classes[i].EntryID;
                _classes[i].Members.Add(_entries[index]);
            }
            else
            {
                int x = 0;
            }
        }

        private void insertEntryToNamespace(int index)
        {
            Entry temp = new Entry();
            temp.TagName = _entries[index].Namespace;
            temp.GenericContext = true;
            temp.Separator = _entries[index].Separator;

            NamespaceContainer cmp = new NamespaceContainer(temp);

            int i = Array.BinarySearch(_namespaces, cmp, _namespaceSorter);

            if (i >= 0)
            {
                _entries[index].ParentID = _namespaces[i].EntryID;
                _namespaces[i].Members.Add(_entries[index]);
            }
        }

        public CtagsParser()
        {
            _entries = new List<Entry>();
            _classes = new ClassContainer[0];
            _namespaces = new NamespaceContainer[0];
        }
    }
}

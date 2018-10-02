using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD
{
    class ERDFormatter
    {
        public List<ERDFormatterNode> Nodes;

        private List<List<int>> _groups;
        private List<SDON.Model.Return> _returns;

        private int _numGroups;

        public void InsertTable(SQLTable table)
        {
            Nodes.Add(new ERDFormatterNode(table, Nodes.Count, this));
        }

        public void Format()
        {
            drawReturns();
            defineTypes();
            groupNodes();
        }

        public SDON.Model.Shape MakeDiagram()
        {
            SDON.Model.Shape root = new SDON.Model.Shape();
            root.ShapeContainer = new SDON.Model.ShapeContainer();
            root.ShapeContainer.Shapes = new List<SDON.Model.Shape>();
            root.ShapeContainer.Arrangement = SDON.Model.ShapeArrangementTypes.Square;
            root.Hide = true;

            SDON.Model.Shape orphanSection = new SDON.Model.Shape();
            orphanSection.ShapeContainer = new SDON.Model.ShapeContainer();
            orphanSection.ShapeContainer.Shapes = new List<SDON.Model.Shape>();
            orphanSection.ShapeContainer.Arrangement = SDON.Model.ShapeArrangementTypes.Square;
            orphanSection.FillColor = "#F7F7F7";
            orphanSection.LineThick = 0;

            SDON.Model.Shape currentColumn;
            SDON.Model.Shape altLeafRow, rootRow, parentRow, leafRow, altRootRow, adjColumn;
            int i, j, adj;

            for (i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].ShapeType == (int)ERDFormatterNodeTypes.Orphan)
                {
                    orphanSection.ShapeContainer.Shapes.Add(Nodes[i].Node.ExportAsShape());
                }
                else
                {
                    Nodes[i].ExclusiveAdjacencyFilter();
                }
            }

            for (i = 0; i < _groups.Count; i++) //function out, man
            {
                if (_groups[i] == null)
                {
                    continue;
                }

                currentColumn = makeShapeContainer(false, true, false);
                rootRow = makeShapeContainer(true);
                parentRow = makeShapeContainer(true);
                leafRow = makeShapeContainer(true);
                altRootRow = makeShapeContainer(true);
                altLeafRow = makeShapeContainer(true);

                for (j = 0; j < _groups[i].Count; j++)
                {
                    if (Nodes[_groups[i][j]].TestExclusiveAdjacency())
                    {
                        continue;
                    }

                    insertReturnsToNode(Nodes[_groups[i][j]]);

                    if (Nodes[_groups[i][j]].ExclusivelyAdjacentChildren.Count > 0)
                    {
                        adjColumn = makeShapeContainer(true, true, false);

                        for (adj = 0; adj < Nodes[_groups[i][j]].ExclusivelyAdjacentChildren.Count; adj += 2)
                        {
                            adjColumn.ShapeContainer.Shapes.Add(Nodes[Nodes[_groups[i][j]].ExclusivelyAdjacentChildren[adj]].Node.ExportAsShape());
                        }

                        addShapeToRow(_groups[i][j], altLeafRow, rootRow, parentRow, leafRow, altRootRow, adjColumn);
                    }

                    addShapeToRow(_groups[i][j], altLeafRow, rootRow, parentRow, leafRow, altRootRow, Nodes[_groups[i][j]].Node.ExportAsShape());

                    if (Nodes[_groups[i][j]].ExclusivelyAdjacentChildren.Count > 1)
                    {
                        adjColumn = makeShapeContainer(true, true, false);

                        for (adj = 1; adj < Nodes[_groups[i][j]].ExclusivelyAdjacentChildren.Count; adj += 2)
                        {
                            adjColumn.ShapeContainer.Shapes.Add(Nodes[Nodes[_groups[i][j]].ExclusivelyAdjacentChildren[adj]].Node.ExportAsShape());
                        }

                        addShapeToRow(_groups[i][j], altLeafRow, rootRow, parentRow, leafRow, altRootRow, adjColumn);
                    }
                }

                if (altRootRow.ShapeContainer.Shapes.Count > 0)
                {
                    currentColumn.ShapeContainer.Shapes.Add(altRootRow);
                }
                if (leafRow.ShapeContainer.Shapes.Count > 0)
                {
                    currentColumn.ShapeContainer.Shapes.Add(leafRow);
                }
                if (parentRow.ShapeContainer.Shapes.Count > 0)
                {
                    currentColumn.ShapeContainer.Shapes.Add(parentRow);
                }
                if (rootRow.ShapeContainer.Shapes.Count > 0)
                {
                    currentColumn.ShapeContainer.Shapes.Add(rootRow);
                }
                if (altLeafRow.ShapeContainer.Shapes.Count > 0)
                {
                    currentColumn.ShapeContainer.Shapes.Add(altLeafRow);
                }

                if (currentColumn.ShapeContainer.Shapes.Count > 0)
                {
                    root.ShapeContainer.Shapes.Add(currentColumn);
                }
            }

            if (orphanSection.ShapeContainer.Shapes.Count > 0)
            {
                root.ShapeContainer.Shapes.Add(orphanSection);
            }

            return root;
        }

        public List<SDON.Model.Return> GetReturns()
        {
            return _returns;
        }

        private SDON.Model.Shape makeShapeContainer(bool hidden = false, bool isColumn = false, bool includeAlign = true)
        {
            SDON.Model.Shape ret = new SDON.Model.Shape();
            ret.ShapeContainer = new SDON.Model.ShapeContainer();
            ret.ShapeContainer.Arrangement = isColumn ? SDON.Model.ShapeArrangementTypes.Column : SDON.Model.ShapeArrangementTypes.Row;
            ret.ShapeContainer.Shapes = new List<SDON.Model.Shape>();

            if (includeAlign)
            {
                ret.ShapeContainer.ShapesAlignV = SDON.Model.VerticalAlignments.Middle;
            }

            if (hidden)
            {
                ret.Hide = true;
            }
            else
            {
                ret.LineThick = 0;
                ret.FillColor = "#F7F7F7";
            }

            return ret;
        }

        private void addShapeToRow(int node, SDON.Model.Shape altLeaves, SDON.Model.Shape roots, SDON.Model.Shape parents, SDON.Model.Shape leaves, SDON.Model.Shape altRoots, SDON.Model.Shape shape)
        {
            if (Nodes[node].ShapeType == (int)ERDFormatterNodeTypes.Root)
            {
                if (Nodes[node].TestIsOnlyConnectedToLeaves()) {
                    altRoots.ShapeContainer.Shapes.Add(shape);
                }
                else
                {
                    roots.ShapeContainer.Shapes.Add(shape);
                }
            }
            else if ((Nodes[node].ShapeType == (int)ERDFormatterNodeTypes.Parent) ||
                (Nodes[node].ShapeType == (int)ERDFormatterNodeTypes.Abomination))
            {
                parents.ShapeContainer.Shapes.Add(shape);
            }
            else if (Nodes[node].ShapeType == (int)ERDFormatterNodeTypes.Leaf)
            {
                if (Nodes[node].TestIsOnlyConnectedToRoots())
                {
                    altLeaves.ShapeContainer.Shapes.Add(shape);
                }
                else
                {
                    leaves.ShapeContainer.Shapes.Add(shape);
                }
            }
        }

        private void drawReturns()
        {
            int i, j;
            int dest;
            string destName;

            for (i = 0; i < Nodes.Count; i++)
            {
                for (j = 0; j < Nodes[i].Node.GetEntryCount(); j++)
                {
                    destName = Nodes[i].Node.GetEntry(j).ChildTable;

                    if (destName != "NULL")
                    {
                        dest = findNodeWithName(destName);
                        if ((dest >= 0) && (dest != i))
                        {
                            Nodes[i].ChildrenIndeces.Add(dest);
                            Nodes[dest].ParentIndeces.Add(i);
                        }
                    }
                }
            }
        }

        private void insertReturnsToNode(ERDFormatterNode node)
        {
            string startDir;
            string endDir;
            int i;

            for (i = 0; i < node.ChildrenIndeces.Count; i++)
            {
                if (!Nodes[node.ChildrenIndeces[i]].TestExclusiveAdjacency())
                {
                    startDir = getNormalRelativeDirection(node.ChildrenIndeces[i], node.Index);
                    endDir = getNormalRelativeDirection(node.Index, node.ChildrenIndeces[i]);
                    insertReturn((int)node.Node.ID, (int)Nodes[node.ChildrenIndeces[i]].Node.ID, startDir, endDir);
                }
            }
            for (i = 0; i < node.ExclusivelyAdjacentChildren.Count; i++)
            {
                if ((i & 0x1) == 0) //Exclusively adjacent children are staggered left and right
                {
                    startDir = SDON.Model.Directions.Left;
                    endDir = SDON.Model.Directions.Right;
                }
                else
                {
                    startDir = SDON.Model.Directions.Right;
                    endDir = SDON.Model.Directions.Left;
                }

                insertReturn((int)node.Node.ID, (int)Nodes[node.ExclusivelyAdjacentChildren[i]].Node.ID, startDir, endDir);
            }
        }

        private string getNormalRelativeDirection(int start, int end)
        {
            int startType = Nodes[start].ShapeType;
            int endType = Nodes[end].ShapeType;

            if (startType == (int)ERDFormatterNodeTypes.Abomination)
            {
                startType = (int)ERDFormatterNodeTypes.Parent;
            }
            if (endType == (int)ERDFormatterNodeTypes.Abomination)
            {
                endType = (int)ERDFormatterNodeTypes.Parent;
            }

            if ((startType == (int)ERDFormatterNodeTypes.Leaf) && Nodes[start].TestIsOnlyConnectedToRoots())
            {
                return SDON.Model.Directions.Bottom;
            }
            else if ((endType == (int)ERDFormatterNodeTypes.Leaf) && Nodes[end].TestIsOnlyConnectedToRoots())
            {
                return SDON.Model.Directions.Top;
            }

            if ((startType == (int)ERDFormatterNodeTypes.Root) && Nodes[start].TestIsOnlyConnectedToLeaves())
            {
                return SDON.Model.Directions.Top;
            }
            else if ((endType == (int)ERDFormatterNodeTypes.Root) && Nodes[end].TestIsOnlyConnectedToLeaves())
            {
                return SDON.Model.Directions.Bottom;
            }

            if (startType < endType)
            {
                return SDON.Model.Directions.Bottom;
            }

            return SDON.Model.Directions.Top;
        }

        private string getReverseDirection(string dir)
        {
            if (dir == SDON.Model.Directions.Left)
            {
                return SDON.Model.Directions.Right;
            }
            if (dir == SDON.Model.Directions.Bottom)
            {
                return SDON.Model.Directions.Top;
            }
            if (dir == SDON.Model.Directions.Right)
            {
                return SDON.Model.Directions.Left;
            }
            return SDON.Model.Directions.Bottom;
        }

        private void insertReturn(int startID, int endID, string startDir, string endDir)
        {
            if (startID == endID)
            {
                return; //Can't draw a return to itself
            }

            for (int i = 0; i < _returns.Count; i++)
            {
                if ((_returns[i].StartID == startID) && (_returns[i].EndID == endID))
                {
                    return;
                }
                if ((_returns[i].StartID == endID) && (_returns[i].EndID == startID))
                {
                    _returns[i].EndArrow = 13;
                    return;
                }
            }

            SDON.Model.Return ret;

            ret = new SDON.Model.Return();
            ret.StartID = startID;
            ret.EndID = endID;
            ret.StartArrow = 13;
            ret.EndArrow = 15;
            ret.StartDirection = startDir;
            ret.EndDirection = endDir;
            ret.LineThick = 2;
            _returns.Add(ret);
        }

        private int findNodeWithName(string name)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Node.GetTableName() == name)
                {
                    return i;
                }
            }

            return -1;
        }

        private void defineTypes()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].DefineType();
            }
        }

        private void groupNodes()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].ParentIndeces.Count > 0)
                {
                    groupWithParents(Nodes[i]);
                }
                if (Nodes[i].ChildrenIndeces.Count > 0)
                {
                    groupWithChildren(Nodes[i]);
                }
            }
        }

        private void groupWithParents(ERDFormatterNode node)
        {
            for (int i = 0; i < node.ParentIndeces.Count; i++)
            {
                if (node.GroupNum >= 0)
                {
                    if (Nodes[node.ParentIndeces[i]].GroupNum >= 0)
                    {
                        mergeGroups(node.GroupNum, Nodes[node.ParentIndeces[i]].GroupNum);
                    }
                    else
                    {
                        Nodes[node.ParentIndeces[i]].GroupNum = node.GroupNum;
                        _groups[node.GroupNum].Add(node.ParentIndeces[i]);
                    }
                }
                else
                {
                    if (Nodes[node.ParentIndeces[i]].GroupNum >= 0)
                    {
                        node.GroupNum = Nodes[node.ParentIndeces[i]].GroupNum;
                        _groups[node.GroupNum].Add(node.Index);
                    }
                    else
                    {
                        node.GroupNum = _groups.Count;
                        Nodes[node.ParentIndeces[i]].GroupNum = _groups.Count;
                        _groups.Add(new List<int>());
                        _groups[node.GroupNum].Add(node.Index);
                        _groups[node.GroupNum].Add(node.ParentIndeces[i]);
                        _numGroups++;
                    }
                }
            }
        }

        private void groupWithChildren(ERDFormatterNode node)
        {
            for (int i = 0; i < node.ChildrenIndeces.Count; i++)
            {
                if (node.GroupNum >= 0)
                {
                    if (Nodes[node.ChildrenIndeces[i]].GroupNum >= 0)
                    {
                        mergeGroups(node.GroupNum, Nodes[node.ChildrenIndeces[i]].GroupNum);
                    }
                    else
                    {
                        Nodes[node.ChildrenIndeces[i]].GroupNum = node.GroupNum;
                        _groups[node.GroupNum].Add(node.ChildrenIndeces[i]);
                    }
                }
                else
                {
                    if (Nodes[node.ChildrenIndeces[i]].GroupNum >= 0)
                    {
                        node.GroupNum = Nodes[node.ChildrenIndeces[i]].GroupNum;
                        _groups[node.GroupNum].Add(node.Index);
                    }
                    else
                    {
                        node.GroupNum = _groups.Count;
                        Nodes[node.ChildrenIndeces[i]].GroupNum = _groups.Count;
                        _groups.Add(new List<int>());
                        _groups[node.GroupNum].Add(node.Index);
                        _groups[node.GroupNum].Add(node.ChildrenIndeces[i]);
                        _numGroups++;
                    }
                }
            }
        }

        private void mergeGroups(int group1, int group2)
        {
            if (group1 == group2)
            {
                return;
            }

            List<int> rmGroup = _groups[group1];
            _groups[group1] = null;

            for (int i = 0; i < rmGroup.Count; i++)
            {
                Nodes[rmGroup[i]].GroupNum = group2;
            }

            _groups[group2].AddRange(rmGroup);
            _numGroups--;
        }

        public ERDFormatter()
        {
            Nodes = new List<ERDFormatterNode>();
            _groups = new List<List<int>>();
            _numGroups = 0;
            _returns = new List<SDON.Model.Return>();
        }
    }
}

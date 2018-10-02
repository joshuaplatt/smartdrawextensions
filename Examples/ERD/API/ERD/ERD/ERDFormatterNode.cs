using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD
{
    enum ERDFormatterNodeTypes
    {
        Orphan,
        Root,
        Parent,
        Abomination,
        Leaf
    }

    class ERDFormatterNode : IComparable<ERDFormatterNode>
    {
        public long ID;
        public int Index;
        public int NumParents;
        public SQLTable Node;
        public List<int> ChildrenIndeces;
        public List<int> SiblingIndeces;
        public List<int> ParentIndeces;
        public ERDFormatter Formatter;

        public int ShapeType;
        public int GroupNum;
        public List<int> ExclusivelyAdjacentChildren;

        private bool? isOnlyConnectedToLeaves;
        private bool? isOnlyConnectedToRoots;

        public int CompareTo(ERDFormatterNode comp)
        {
            return ID.CompareTo(comp.ID);
        }

        public void DefineType()
        {
            if (ParentIndeces.Count > 0)
            {
                if (ParentIndeces.Count > 1)
                {
                    if (ChildrenIndeces.Count == 0)
                    {
                        ShapeType = (int)ERDFormatterNodeTypes.Leaf;
                    }
                    else
                    {
                        ShapeType = (int)ERDFormatterNodeTypes.Abomination;
                    }
                }
                else if (ChildrenIndeces.Count > 0)
                {
                    ShapeType = (int)ERDFormatterNodeTypes.Parent;
                }
                else
                {
                    ShapeType = (int)ERDFormatterNodeTypes.Leaf;
                }
            }
            else
            {
                if (ChildrenIndeces.Count > 0)
                {
                    ShapeType = (int)ERDFormatterNodeTypes.Root;
                }
                else
                {
                    ShapeType = (int)ERDFormatterNodeTypes.Orphan;
                }
            }
        }

        public void ExclusiveAdjacencyFilter()
        {
            if (TestExclusiveAdjacency())
            {
                Formatter.Nodes[ChildrenIndeces[0]].ExclusivelyAdjacentChildren.Add(Index);
            }
        }

        public bool TestExclusiveAdjacency()
        {
            if ((ChildrenIndeces.Count == 1) && (ParentIndeces.Count == 0))
            {
                return true;
            }
            return false;
        }

        public bool TestIsOnlyConnectedToLeaves()
        {
            if(isOnlyConnectedToLeaves != null)
            {
                return (bool)isOnlyConnectedToLeaves;
            }
            
            for (int i = 0; i < ChildrenIndeces.Count; i++)
            {
                if (Formatter.Nodes[ChildrenIndeces[i]].ShapeType != (int)ERDFormatterNodeTypes.Leaf)
                {
                    isOnlyConnectedToLeaves = false;
                    return false;
                }
            }

            isOnlyConnectedToLeaves = true;
            return true;
        }

        public bool TestIsOnlyConnectedToRoots()
        {
            if (isOnlyConnectedToRoots != null)
            {
                return (bool)isOnlyConnectedToRoots;
            }

            for (int i = 0; i < ParentIndeces.Count; i++)
            {
                if (Formatter.Nodes[ParentIndeces[i]].ShapeType != (int)ERDFormatterNodeTypes.Root)
                {
                    isOnlyConnectedToRoots = false;
                    return false;
                }
            }

            isOnlyConnectedToRoots = true;
            return true;
        }

        public ERDFormatterNode(SQLTable table, int index, ERDFormatter formatter)
        {
            Node = table;
            ID = -1;
            NumParents = 0;
            Index = index;
            ChildrenIndeces = new List<int>();
            SiblingIndeces = new List<int>();
            ParentIndeces = new List<int>();
            Formatter = formatter;
            GroupNum = -1;
            ExclusivelyAdjacentChildren = new List<int>();
            isOnlyConnectedToLeaves = null;
            isOnlyConnectedToRoots = null;
        }
    }
}

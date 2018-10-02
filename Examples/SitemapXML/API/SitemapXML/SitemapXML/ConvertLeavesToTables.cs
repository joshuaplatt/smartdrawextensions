using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitemapXML
{
    class ConvertLeavesToTables
    {
        private SitemapFilter _converter;
        private SDON.Model.Diagram _root;

        public void Convert()
        {
            if (_root.Shape != null)
            {
                _converter.outputToConsole("Converting to tables...");
                recursiveLeavesToTables(_root.Shape);
            }
        }

        private void recursiveLeavesToTables(SDON.Model.Shape currentShape)
        {
            if (checkIfShapeHasMultipleLeaves(currentShape))
            {
                SDON.Model.Shape temp = generateTableWithLeaves(currentShape);
                clearLeavesInShape(currentShape, temp);
            }

            if(currentShape.ShapeConnector == null)
            {
                return;
            }

            foreach (SDON.Model.ShapeConnector connector in currentShape.ShapeConnector)
            {
                foreach (SDON.Model.Shape shape in connector.Shapes)
                {
                    recursiveLeavesToTables(shape);
                }
            }
        }

        private bool checkIfShapeHasMultipleLeaves(SDON.Model.Shape shape)
        {
            int count = 0;

            if(shape.ShapeConnector == null)
            {
                return false;
            }

            foreach (SDON.Model.ShapeConnector connector in shape.ShapeConnector)
            {
                foreach (SDON.Model.Shape child in connector.Shapes)
                {
                    if ((child.ShapeConnector == null) || (child.ShapeConnector.Count == 0))
                    {
                        count++;
                        if (count >= 2)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private SDON.Model.Shape generateTableWithLeaves(SDON.Model.Shape shape)
        {
            SDON.Model.Shape ret = new SDON.Model.Shape();
            SDON.Model.Cell tempCell;
            ret.Table = new SDON.Model.Table();
            ret.Table.Cell = new List<SDON.Model.Cell>();
            ret.Table.Rows = 0;
            ret.TextMargin = 6;
            ret.TextGrow = SDON.Model.TextGrow.Horizontal;
            ret.ShapeType = "Rect";
            ret.FillColor = "#F2F2F2";
            ret.LineColor = "#BFBFBF";

            if(shape.ShapeConnector == null)
            {
                return null;
            }

            SDON.Model.ShapeConnector connector = shape.ShapeConnector.FirstOrDefault();

            if (connector == null)
            {
                return null;
            }

            foreach (SDON.Model.Shape child in connector.Shapes)
            {
                if (child.ShapeConnector == null)
                {
                    tempCell = new SDON.Model.Cell();
                    tempCell.Label = child.Label;
                    tempCell.TextAlignH = SDON.Model.HorizontalAlignments.Left;
                    tempCell.Truncate = 72;

                    if (_converter.GenerateHyperlinks)
                    {
                        tempCell.Hyperlink = new SDON.Model.Hyperlink();
                        tempCell.Hyperlink = child.Hyperlink;
                    }

                    tempCell.Row = ret.Table.Rows + 1;
                    ret.Table.Cell.Add(tempCell);
                    ret.Table.Rows++;
                }
            }

            //temp <- pretty sure this was fixed?
            if (_converter.GenerateHyperlinks)
            {
                ret.Hyperlink = new SDON.Model.Hyperlink();
                ret.Hyperlink.url = "";
            }
            
            return ret;
        }

        private void clearLeavesInShape(SDON.Model.Shape shape, SDON.Model.Shape table)
        {
            if((table == null) || (shape.ShapeConnector == null) || (shape.ShapeConnector.Count == 0))
            {
                return;
            }

            List<SDON.Model.Shape> oldShapes = shape.ShapeConnector[0].Shapes;
            shape.ShapeConnector[0].Shapes = new List<SDON.Model.Shape>();
            SDON.Model.Shape newShape = table;

            foreach (SDON.Model.Shape child in oldShapes)
            {
                if (child.ShapeConnector != null)
                {
                    shape.ShapeConnector[0].Shapes.Add(child);
                }
            }

            shape.ShapeConnector[0].Shapes.Add(newShape);
        }

        public ConvertLeavesToTables(SDON.Model.Diagram root, SitemapFilter converter)
        {
            this._converter = converter;
            this._root = root;
        }
    }
}

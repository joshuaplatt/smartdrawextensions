using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitemapXML
{
    class InsertURIToRoot
    {
        private SDON.Model.Diagram _root;
        private int _maxDepth;
        private int _maxShapes;
        private int _numShapes;
        private SitemapFilter _converter;

        private const string FolderShape = "Parent";
        private const string LeafShape = "Leaf";
        private const string NormalShape = "Parent";

        public void InsertURI(Uri url, string lastMod, ref int numShapes)
        {
            string[] path = url.Segments;
            SDON.Model.Shape shape = _root.Shape;
            this._numShapes = numShapes;

            if ((path.Length == 0) || (numShapes >= _maxShapes))
            {
                return;
            }

            if (shape == null)
            {
                shape = new SDON.Model.Shape();
                shape.TextGrow = SDON.Model.TextGrow.Horizontal;
                shape.Truncate = 72;
                shape.Label = url.Host;
                shape.ShapeType = NormalShape;

                if (_converter.GenerateHyperlinks)
                {
                    shape.Hyperlink = new SDON.Model.Hyperlink();
                    shape.Hyperlink.url = "http://" + url.Host;
                }

                _root.Shape = shape;
            }

            if ((lastMod != "") && (_converter.ExcludeBefore != null))
            {
                DateTime dt = new DateTime();
                dt = Convert.ToDateTime(lastMod);
                if (dt.CompareTo(_converter.ExcludeBefore) < 0)
                {
                    return;
                }
            }

            insertShapes(path, shape, url);

            numShapes = this._numShapes;
        }

        private void insertShapes(string[] path, SDON.Model.Shape rootShape, Uri url)
        {
            int i = ((path.Length > 0) && (path[0] == "/")) ? 1 : 0;
            SDON.Model.Shape parent = rootShape;
            SDON.Model.Shape child = null;
            string currentStep;

            for (; (i < path.Length) && (i < _maxDepth); i++)
            {
                if ((path[i].Length > 0) && (path[i][path[i].Length - 1] == '/'))
                {
                    if (path[i].Length == 1)
                    {
                        break;
                    }

                    currentStep = path[i].Substring(0, path[i].Length - 1);
                }
                else
                {
                    currentStep = path[i];
                }

                child = FilterUtility.SearchConnectedShapesForLabel(parent, currentStep);

                if (child == null)
                {
                    child = new SDON.Model.Shape();
                    child.TextGrow = SDON.Model.TextGrow.Horizontal;
                    child.Truncate = 72;
                    child.Label = currentStep;

                    if (((i + 1) == path.Length) || (((i + 2) == path.Length) && (path[path.Length - 1] == "/")))
                    {
                        // --temp-- FOR ALL SITUATIONS WHERE SHAPETYPE IS MENTIONED:
                        //There will only be 2 types of shapes:
                        // 1: Endpoint (where a link will point to)
                        // 2: Directory (where there will be no link, just a directory section)
                        child.ShapeType = LeafShape;

                        if (_converter.GenerateHyperlinks)
                        {
                            child.Hyperlink = new SDON.Model.Hyperlink();
                            child.Hyperlink.url = url.OriginalString;
                        }
                    }
                    else
                    {
                        child.ShapeType = FolderShape;
                    }
                    
                    FilterUtility.InsertConnectedShape(parent, child);
                    _numShapes++;

                    if (parent.ShapeType == LeafShape)
                    {
                        parent.ShapeType = NormalShape;
                    }

                    if (_numShapes >= _maxShapes)
                    {
                        _converter.outputToConsole("Reached maximum number of shapes");
                        break;
                    }
                }
                else if ((child.ShapeType == FolderShape) && (((i + 1) == path.Length) || (((i + 2) == path.Length) && (path[path.Length - 1] == "/"))))
                {
                    child.ShapeType = NormalShape;

                    if (_converter.GenerateHyperlinks)
                    {
                        child.Hyperlink = new SDON.Model.Hyperlink();
                        child.Hyperlink.url = url.OriginalString;
                    }
                }

                parent = child;
                child = null;
            }
        }

        public InsertURIToRoot(SDON.Model.Diagram root, int maxDepth, int maxShapes, SitemapFilter converter)
        {
            this._root = root;
            this._maxDepth = maxDepth;
            this._maxShapes = maxShapes;
            this._numShapes = 0;
            this._converter = converter;
        }
    }
}

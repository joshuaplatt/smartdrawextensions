using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitemapXML
{
    abstract class FilterUtility
    {
        static public void InsertConnectedShape(SDON.Model.Shape parent, SDON.Model.Shape child)
        {
            if((parent == null) || (child == null))
            {
                return;
            }

            SDON.Model.ShapeConnector con;

            if ((parent.ShapeConnector == null) || (parent.ShapeConnector.Count == 0))
            {
                con = new SDON.Model.ShapeConnector();
                con.Direction = SDON.Model.Directions.Bottom;
                con.Shapes = new List<SDON.Model.Shape>();
                con.DefaultShape = new SDON.Model.Shape();
                con.DefaultShape.MinWidth = 151;
                con.DefaultShape.MinHeight = 76;
                con.DefaultShape.ShapeType = "Rect";

                parent.ShapeConnector = new List<SDON.Model.ShapeConnector>();
                parent.ShapeConnector.Add(con);
            }
            else
            {
                con = parent.ShapeConnector[0];
            }
            
            con.Shapes.Add(child);
        }

        static public SDON.Model.Shape SearchConnectedShapesForLabel(SDON.Model.Shape parent, string label)
        {
            if((parent == null) || (label == null) || (parent.ShapeConnector == null))
            {
                return null;
            }

            int i, j;
            SDON.Model.ShapeConnector tempCon;

            for(i = 0; i < parent.ShapeConnector.Count; i++)
            {
                tempCon = parent.ShapeConnector[i];
                
                if(tempCon.Shapes == null)
                {
                    continue;
                }

                for(j = 0; j < tempCon.Shapes.Count; j++)
                {
                    if(tempCon.Shapes[j].Label == label)
                    {
                        return tempCon.Shapes[j]; //found it
                    }
                }
            }

            return null;
        }
    }
}

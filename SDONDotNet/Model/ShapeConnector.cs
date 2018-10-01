using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SDON.Serialization;

namespace SDON.Model
{
    /// <summary>
    /// A ShapeConnector contains an array of shapes that are connected to it by an automatic connector. Defines an automatic connector.
    /// </summary>
    [Serializable]
    public sealed class ShapeConnector : SDONSerializeable
    {
        [DataMember(Name = "Collapse")]
        [IgnoreIfDefaultValue(Default = false)]
        private bool _collapse = false;

        [DataMember(Name = "Direction")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _direction = null;

        [DataMember(Name = "LineThick")]
        [IgnoreIfDefaultValue(Default = -1.0)]
        private double _lineThick = -1.0;

        [DataMember(Name = "LineColor")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _lineColor = null;

        [DataMember(Name = "Arrangement")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _arrangement = null;

        [DataMember(Name = "Shapes")]
        [IgnoreIfDefaultValue(Default = null)]
        private List<Shape> _shapes = null;

        [DataMember(Name = "ShapeConnectorType")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _shapeConnectorType = null;


        [DataMember(Name = "StartArrow")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _startArrow = -1;

        [DataMember(Name = "EndArrow")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _endArrow = -1;

        /// <summary>
        /// Whether or not to collapse (hide) the connector. The connector is collapsed initially. This applies only to tree-like diagrams (not flowcharts).
        /// </summary>
        public bool Collapse
        {
            get { return _collapse; }
            set { _collapse = value; }
        }

        /// <summary>
        /// The direction of the connector from the parent shape. 
        /// 
        /// For Mind Maps this can be Left or Right for the ShapeLists connected to the root shape. All other uses are ignored. The default is “Right”. Mind Maps ignore any more than two ShapeLists for the root shape and  any more than one for other shapes.
        /// 
        /// For Org Charts (trees) the first ShapeList connected to the root shape can be in any direction and this sets the direction of the tree. The default is “Down”. All other values are ignored. Org charts ignore any more than one ShapeList per shape.
        /// 
        /// For Flow Charts any shape can have multiple ShapeLists in any direction. If two or more ShapeLists attached to a single shape have the same direction, they are shown as a split path. The default direction for a ShapeList is “Right”.
        /// </summary>
        public string Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// The thickness of the line in 1/100”. If omitted, the thickness is the default for the template.
        /// </summary>
        public double LineThick
        {
            get { return _lineThick; }
            set { _lineThick = value; }
        }

        /// <summary>
        /// The line color of the connector as a hex RGB value. If omitted, the color is the default for the template.
        /// </summary>
        public string LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        public string Arrangement
        {
            get { return _arrangement; }
            set { _arrangement = value; }
        }

        /// <summary>
        /// A list of shapes that are attached to the connector. The shapes are attached in the order they appear in in the list.
        /// </summary>
        public List<Shape> Shapes
        {
            get { return _shapes; }
            set { _shapes = value; }
        }

        /// <summary>
        /// Defines the way the shape connector will be formatted.
        /// </summary>
        public string ShapeConnectorType
        {
            get { return _shapeConnectorType; }
            set { _shapeConnectorType = value; }
        }

        /// <summary>
        /// The arrowhead that will appear on the beginning of the line.
        /// </summary>
        public int StartArrow
        {
            get { return _startArrow; }
            set { _startArrow = value; }
        }

        /// <summary>
        /// The arrowhead that will appear on the end of the line.
        /// </summary>
        public int EndArrow
        {
            get { return _endArrow; }
            set { _endArrow = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShapeConnector()
        {
        }

        /// <summary>
        /// Special constructor for implementing ISerializeable. Deserializes the object.
        /// </summary>
        /// <param name="info">Serialization info (all the values that are on the incoming object graph) of the object being deserialized.</param>
        /// <param name="context">The deserialization context.</param>
        internal ShapeConnector(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

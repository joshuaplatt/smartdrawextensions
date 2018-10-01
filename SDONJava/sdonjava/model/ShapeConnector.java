package sdonjava.model;

import java.util.ArrayList;

import sdonjava.serialization.SDONSerializable;

/**
 * A ShapeConnector contains an array of shapes that are
 * connected to it by an automatic connector.  Defines an
 * automatic connector.
 */
public class ShapeConnector {
    //Mention: False default?
    /**
     * Whether or not to collapse (hide) the connector.
     * The connector is collapsed initially.  This applies
     * only to tree-like diagrams (not Flowcharts).
     */
    @SDONSerializable
    public Boolean Collapse = null;
    
    /**
     * The direction of the connector from the parent shape.
     * <ul><li>
     * For Mind Maps, this can be Left or Right for the
     * ShapeLists connected to the root shape.  All other
     * uses are ignored.  The default is "Right".  Mind
     * Maps ignore any more than two ShapeLists for the
     * root shape and any more than one for other shapes.
     * </li><li>
     * For Org Charts (trees), the first ShapeList
     * connected to the root shape can be in any direction
     * and this sets the direction of the tree.  The
     * default is "Down".  All other values are ignored.
     * Org Charts ignore any more than one ShapeList per
     * shape.
     * </li><li>
     * For Flowcharts, any shape can have multiple
     * ShapeLists in any direction.  If two or more Shape
     * Lists attached to a single shape have the same
     * direction, they are shown as a split path.  The
     * default direction for a ShapeList is "Right".
     * </li></ul>
     * @see {@link Directions}
     */
    @SDONSerializable
    public String Direction = null;
    
    /**
     * The thickness of the line in 1/100". If omitted, the
     * thickness is default for the template.
     */
    @SDONSerializable
    public double LineThick = -1.0;
    
    /**
     * The line color of the connector as a hex RGB value.
     * If omitted, the color is the default for the
     * template.
     */
    @SDONSerializable
    public String LineColor = null;
    
    /**
     * The default properties for any shape on this connector.
     */
    @SDONSerializable
    public Shape DefaultShape = new Shape();
    
    /**
     * A list of shapes that are attached to the connector.
     * The shapes are attached in the order they appear in
     * this list.
     */
    @SDONSerializable
    public ArrayList<Shape> Shapes = new ArrayList<Shape>();
}
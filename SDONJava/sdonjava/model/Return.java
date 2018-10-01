package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * A segmented line that links two shapes.
 */
public class Return {
    /**
     * The ID defined for the starting shape by its "ID"
     * property.
     * @see {@link Shape#ID}
     */
    @SDONSerializable
    public int StartID = -1;
    
    /**
     * The ID defined for the ending shape by its "ID"
     * property (see StartID).  The starting and ending
     * shapes define the direction of the line as far as
     * any arrowhead is concerned, the arrowhead touches
     * the ending shape.
     * @see {@link Shape#ID}
     */
    @SDONSerializable
    public int EndID = -1;
    
    /**
     * The direction the connector leaves the parent shape.
     * The default is Down.  A value from the
     * {@link Directions} enum.
     */
    @SDONSerializable
    public String StartDirection = null;
    
    /**
     * The direction the line takes out of the ending
     * shape.  The default is Down.  A value from the
     * {@link Directions} enum.
     */
    @SDONSerializable
    public String EndDirection = null;
    
    /**
     * The pattern of the line connecting the shapes.  A
     * value from the {@link LinePatterns} enum.
     */
    @SDONSerializable
    public String LinePattern = null;
    
    /**
     * The text that appears on the line.
     */
    @SDONSerializable
    public String Label = null;
    
    /**
     * By default returns have an arrowhead touching the
     * end shape.  This property can be turned off by using
     * 0, or it can change the arrowhead from the default.
     * @deprecated See: {@link #StartArrow},
     * {@link #EndArrow}
     */
    @SDONSerializable
    public int Arrowhead = -1;
    
    /**
     * The arrowhead that will appear on the beginning of
     * the line.  Default 0 (disabled), can be set to 1
     * (normal) or another arrowhead type from
     * {@link ArrowheadTypes}.
     */
    @SDONSerializable
    public int StartArrow = -1;
    
    /**
     * The arrowhead that will appear on the end of the
     * line.  Default 1 (normal), can be set to 0
     * (disabled) or another arrowhead type from
     * {@link ArrowheadTypes}.
     */
    @SDONSerializable
    public int EndArrow = -1;
    
    /**
     * The thickness of the line in 1/100".  Otherwise, the
     * thickness is default for the template.
     */
    @SDONSerializable
    public double LineThick = -1.0;
    
    /**
     * Make the line color of the connector the specified
     * RGB value.  Otherwise, the color is the default for
     * the template.
     */
    @SDONSerializable
    public String LineColor = null;
    
    /**
     * Represents if the return should be curved or not.
     * @since SDON 1.2
     */
    @SDONSerializable
    public Boolean Curved = null;
}
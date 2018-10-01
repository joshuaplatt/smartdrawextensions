package sdonjava.model;

import java.util.ArrayList;

import sdonjava.serialization.SDONSerializable;

/**
 * Represents a shape in a SmartDraw diagram.
 */
public class Shape {
    /**
     * The ID of this shape.  IDs are used to specify the
     * paths of Return lines.  IDs are arbitrary but should
     * be unique.
     * @see {@link Return#StartID}
     * @see {@link Return#EndID}
     */
    @SDONSerializable
    public int ID = -1;
    
    /**
     * The text label inside the shape.
     */
    @SDONSerializable
    public String Label = null;
    
    /**
     * The type of shape.  Used to change the shape from
     * rectangle (default) to another shape.  A value from
     * the {@link ShapeTypes} enum.
     */
    @SDONSerializable
    public String ShapeType = null;
    
    /**
     * Makes the text label be bold with a value of true,
     * not bold with false.  If omitted, boldness follows
     * the template default.
     */
    @SDONSerializable
    public Boolean TextBold = null;
    
    /**
     * Makes the text label be italic witha  value of true,
     * not italic with false.  If omitted, italic follows
     * the template default.
     */
    @SDONSerializable
    public Boolean TextItalic = null;
    
    /**
     * Makes the text label be underlined wiht a value of
     * true, not underlined with false.  If omitted,
     * underlined follows the template default.
     */
    @SDONSerializable
    public Boolean TextUnderline = null;
    
    /**
     * The specified value represents the point size of the
     * text label.  If omitted, the text size is the
     * default for the template.
     */
    @SDONSerializable
    public double TextSize = -1.0;
    
    /**
     * The specified value represents the font of the text
     * label.  If omitted, the font is the default for the
     * template.  Any font can be defined, but will fall
     * back to the system default if the font is
     * unavailable.
     */
    @SDONSerializable
    public String TextFont = null;
    
    /**
     * The color of the text label of the shape (hex RGB
     * value).  If omitted, the color is the default for
     * the template.
     */
    @SDONSerializable
    public String TextColor = null;
    
    /**
     * Aligns the text label to the left, right or centered
     * in the shape.  If omitted, the aligment is the
     * default for the template.  A value from the
     * {@link HorizontalAlignments} enum.
     */
    @SDONSerializable
    public String TextAlignH = null;
    
    /**
     * Aligns the text label to the top, bottom or middle
     * in the shape.  If omitted, the alignment is the
     * defalt for the template.  A value from the
     * {@link VerticalAlignments} enum.
     */
    @SDONSerializable
    public String TextAlignV = null;
    
    /**
     * Defines the number of characters to allow in a shape
     * or cell before the remaining text is truncated.  By
     * default, nothing is truncated.  Defining
     * TextTruncate to "-1" turns it off if on by default.
     * @deprecated See: {@link #Truncate}
     */
    @SDONSerializable
    public int TextTruncate = -1;
    
    /**
     * Determines the direction in which the text will grow
     * in a shape.  Values must be from the
     * {@link TextGrow} enum.
     */
    @SDONSerializable
    public String TextGrow = null;
    
    /**
     * The fill color of the shape (hex RGB value).  If
     * omitted, color is the default for the template.
     */
    @SDONSerializable
    public String FillColor = null;
    
    /**
     * The border thickness of the shape in 1/100".  If
     * omitted, the thickness is the default for the
     * template.
     */
    @SDONSerializable
    public double LineThick = -1.0;
    
    /**
     * The border color of the shape (hex RGB value).  If
     * omitted, color is the default for the template.
     */
    @SDONSerializable
    public String LineColor = null;
    
    /**
     * A text label on the connector line segment that
     * touches the shape.  Only applies to the shape.
     */
    @SDONSerializable
    public String LineLabel = null;
    
    /**
     * Whether or not to display the parent shape of a
     * ShapeArray.
     * @since SDON 1.2
     */
    @SDONSerializable
    public Boolean Hide = null;
    
    /**
     * The number of characters to truncate text after.
     */
    @SDONSerializable
    public int Truncate = -1;
    
    /**
     * The list of ShapeConnectors that are attached to and
     * branch off from this shape.
     */
    @SDONSerializable
    public ArrayList<ShapeConnector> ShapeConnector = new ArrayList<ShapeConnector>();
    
    /**
     * A hyperlink on the shape.
     */
    @SDONSerializable
    public TextHyperlink Hyperlink = new TextHyperlink();
    
    /**
     * Divides the shape up into rows and columns.
     */
    @SDONSerializable
    public Table Table = new Table();
    
    /**
     * An image put into the shape.
     */
    @SDONSerializable
    public Image Image = new Image();
    
    /**
     * Represents a grid-like formation for a group of shapes.
     * @since SDON 1.2
     */
    @SDONSerializable
    public ShapeArray ShapeArray = new ShapeArray();
    
    /**
     * This value represents the minimum width of the shape
     * in 1/100".  The text in the shape may force the
     * width to be wider than this value.
     * @since SDON 1.2
     */
    @SDONSerializable
    public double MinWidth = -1.0;
    
    /**
     * This value represents the minimum height of the
     * shape in 1/100".  The text in the shape may force
     * the height to be taller than this value.
     * @since SDON 1.2
     */
    @SDONSerializable
    public double MinHeight = -1.0;
    
    /**
     * Specifies the way the shape connector will be
     * formatted.  A value from the
     * {@link ShapeConnectorTypes} enum.
     * @since SDON 1.2
     */
    @SDONSerializable
    public String ShapeConnectorType = null;
}
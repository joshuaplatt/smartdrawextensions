package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * Represets a Cell in a table.
 */
public class Cell {
    /**
     * The column of the cell.  Note that the first column
     * is column 1, not column 0.
     */
    @SDONSerializable
    public int Column = -1;
    
    /**
     * The row of the cell.  Note that the first row is row
     * 1, not row 0.
     */
    @SDONSerializable
    public int Row = -1;
    
    /**
     * The text label insite the cell.
     */
    @SDONSerializable
    public String Label = null;
    
    /**
     * The specified value represents the point size of the
     * text label.  If omitted, the text size is the
     * default for the template.
     */
    @SDONSerializable
    public double TextSize = -1.0;
    
    /**
     * Makes the text label be bold with a value of true,
     * not bold with false.  If omitted, boldness follows
     * the template default.
     */
    @SDONSerializable
    public Boolean TextBold = null;
    
    /**
     * Makes the text label be italic witha value of true,
     * not italic with false.  If omitted, italic follows
     * the template default.
     */
    @SDONSerializable
    public Boolean TextItalic = null;
    
    /**
     * Makes the text label be underlined with a value of
     * true, not underlined with false.  If omitted,
     * underline follows the template default.
     */
    @SDONSerializable
    public Boolean TextUnderline = null;
    
    /**
     * The specified value represents the font of the text
     * label.  If omitted, the font is default for the
     * template.  Any font can be defined, but will fall
     * back to system default if font is unavailable.
     */
    @SDONSerializable
    public String TextFont = null;
    
    /**
     * The color of the text label of the cell (hex RGB
     * value).  If omitted, the color is the default for
     * the template.
     */
    @SDONSerializable
    public String TextColor = null;
    
    /**
     * The fill color of the shape (hex RGB value).  If
     * omitted, color is default for the template.
     */
    @SDONSerializable
    public String FillColor = null;
    
    /**
     * Aligns the text label to the left, right or centered
     * in the shape.  If omitted, the alignment is default
     * for the template.  A value from the
     * {@link HorizontalAlignments} enum.
     */
    @SDONSerializable
    public String TextAlignH = null;
    
    /**
     * Aligns the text label to the top, bottom or middle
     * in the shape.  If omitted, the alignment is default
     * for the template.  A value from the
     * {@link VerticalAlignments} enum.
     */
    @SDONSerializable
    public String TextAlignV = null;
    
    /**
     * The number of characters to truncate text after.
     */
    @SDONSerializable
    public int Truncate = -1;
    
    /**
     * A hyperlink in the cell.
     */
    @SDONSerializable
    public TextHyperlink Hyperlink = new TextHyperlink();
    
    /**
     * Defines the url to the image to be shown in the cell.
     */
    @SDONSerializable
    public Image ImageURL = new Image();
    
    /**
     * The shape within the cell if this is in a Shape Table.
     * @since SDON 1.2
     */
    @SDONSerializable
    public Shape Shape = new Shape();
}
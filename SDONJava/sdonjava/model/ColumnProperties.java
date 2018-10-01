package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * The properties of a column.
 */
public class ColumnProperties {
    /**
     * The index of the column.
     */
    @SDONSerializable
    public int Index = -1;
    
    /**
     * The thickness of the column borders in 1/100".
     * Otherwise, the thickness is default for the
     * template.
     */
    @SDONSerializable
    public double LineThick = -1.0;
    
    /**
     * The line color of the column borders as a hex RGB
     * value.  If omitted, the color is the default for the
     * template.
     */
    @SDONSerializable
    public String LineColor = null;
    
    /**
     * The minimum height of the column in 1/100".  The
     * text in cells in the row may force the row to be
     * taller tan this height.
     */
    @SDONSerializable
    public double Height = -1.0;
    
    /**
     * The desired width of the column in 1/100". Note any
     * change n in column N's width reduces the width of
     * column N+1 by the same amount.
     */
    @SDONSerializable
    public double Width = -1.0;
}
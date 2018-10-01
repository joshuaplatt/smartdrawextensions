package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * The properties of a row.
 */
public class RowProperties {
    /**
     * The index of the row.
     * @deprecated SDON 1.2
     * @see {@link #Row} (SDON 1.2)
     */
    @SDONSerializable
    public int Index = -1;
    
    /**
     * The index of the row.
     * @since SDON 1.2
     */
    @SDONSerializable
    public int Row = -1;
    
    /**
     * The thickness of the row borders in 1/100".  If
     * omitted, the thickness is the default for the
     * template.
     */
    @SDONSerializable
    public double LineThick = -1.0;
    
    /**
     * The line color of the row borders as a hex RGB
     * value.  If omitted, the color is default for the
     * template.
     */
    @SDONSerializable
    public String LineColor = null;
    
    /**
     * The minimum height of the row in 1/100".  The text
     * in cells in the row may force the row to be
     * taller than this height.
     */
    @SDONSerializable
    public double Height = -1.0;
    
    /**
     * The desired width of the row in 1/100".  Note any
     * n in column N's width reduces the width of the
     * column N+1 by the same amount.
     */
    @SDONSerializable
    public double Width = -1.0;
}
package sdonjava.model;

import java.util.ArrayList;

import sdonjava.serialization.SDONSerializable;

/**
 * Represents an array of shapes arranged in a grid.
 * @since SDON 1.2
 */
public class ShapeArray {
    /**
     * The array of shapes to include in the group.
     */
    @SDONSerializable
    public ArrayList<Shape> Shapes = new ArrayList<Shape>();
    
    /**
     * The way shapes are arranged in a group.  Must be a
     * value from {@link ShapeArrangementTypes}.
     */
    @SDONSerializable
    public String Arrangement = null;
    
    /**
     * The spacing between columns in 1/100".
     */
    @SDONSerializable
    public double VerticalSpacing = -1.0;
    
    /**
     * The spacing between rows in 1/100".
     */
    @SDONSerializable
    public double HorizontalSpacing = -1.0;
    
    /**
     * The maximum number of rows for a "Row" arrangement
     * before it wraps to a new row, or the maximum number
     * of columns for a "Column" arrangement before it
     * wraps to a new column.
     */
    @SDONSerializable
    public int Wrap = -1;
    
    /**
     * Controls the positioning of a shape in a column of
     * shapes.  Must be a value from
     * {@link HorizontalAlignments}.
     */
    @SDONSerializable
    public String ArrayAlignH = null;
    
    /**
     * Controls the positioning of a shape in a row of
     * shapes.  Must be a value from
     * {@link VerticalAlignments}
     */
    @SDONSerializable
    public String ArrayAlignV = null;
}
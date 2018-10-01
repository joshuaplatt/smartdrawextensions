package sdonjava.model;

import java.util.ArrayList;

import sdonjava.serialization.SDONSerializable;

/**
 * Represents a table within a Shape.
 */
public class Table {
    /**
     * The number of rows in the table.  This can be
     * omitted for a default of 1 if there are colums
     * defined.  If neither the number of rows nor columns
     * are defined, the Table object is ignored.
     */
    @SDONSerializable
    public int Rows = -1;
    
    /**
     * The number of columns in the table.  This can be
     * omitted for a default of 1 if there are rows
     * defined.  If neither the number of rows nor columns
     * are defined, the Table object is ignored.
     */
    @SDONSerializable
    public int Columns = -1;
    
    /**
     * Specific properties for individual cells.
     * @deprecated SDON 1.2
     * @see {@link #Cells} (SDON 1.2)
     */
    @SDONSerializable
    public ArrayList<Cell> Cell = new ArrayList<Cell>();
    
    /**
     * Specific properties for individual cells.
     * @since SDON 1.2
     */
    @SDONSerializable
    public ArrayList<Cell> Cells = new ArrayList<Cell>();
    
    /**
     * Special properties of individual rows.
     */
    @SDONSerializable
    public ArrayList<RowProperties> RowProperties = new ArrayList<RowProperties>();
    
    /**
     * Special properties of individual columns.
     */
    @SDONSerializable
    public ArrayList<ColumnProperties> ColumnProperties = new ArrayList<ColumnProperties>();
    
    /**
     * List of elements that define a range of table cells
     * to join into one cell.
     */
    @SDONSerializable
    public ArrayList<Join> Join = new ArrayList<Join>();
    
    /**
     * Sets the minimum width of all columns to a value in
     * 1/100".  The text in cells in the column may force
     * the columns to be wider than this width.
     * @since SDON 1.2
     */
    @SDONSerializable
    public double ColumnWidth = -1.0;
    
    /**
     * Sets the minimum height of all row to a value in
     * 1/100".  The text in cells in the row may force the
     * rows to be taller than this height.
     * @since SDON 1.2
     */
    @SDONSerializable
    public double RowHeight = -1.0;
    
    /**
     * Sets an alternating color for all rows within the
     * table.  Set to a new blank AlternateRows object to
     * have it default to 
     * @since SDON 1.2
     */
    @SDONSerializable
    public AlternateRows AlternateRows = new AlternateRows();
}
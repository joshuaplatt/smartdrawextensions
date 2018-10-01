package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * Allows cells within a table to be joined together.
 */
public class Join {
    /**
     * The row of the first cell to be joined to others to
     * the right or below it.  Note that the first row is
     * row 1, not row 0.
     */
    @SDONSerializable
    public int Row = -1;
    
    /**
     * The column of the first cell to be joined to others
     * to the right or below it.  Note that the first
     * column is column 1, not column 0.
     */
    @SDONSerializable
    public int Column = -1;
    
    /**
     * The number of cells to join to the first cell.  If
     * this exceeds the number of cells available within a
     * row or column, the number is reduced to the maximum
     * possible.
     */
    @SDONSerializable(defaultInt = 1)
    public int N = 1;
    
    /**
     * Adding this parameter with a non-zero value makes
     * the join happen down the column.
     */
    @SDONSerializable(defaultInt = 1)
    public int Down = 1;
}
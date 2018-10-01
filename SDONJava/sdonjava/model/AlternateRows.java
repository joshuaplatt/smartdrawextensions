package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * Sets an alternating color to rows within a table.
 * @since SDON 1.2
 */
public class AlternateRows {
    /**
     * The hex RGB color of all odd rows.
     */
    @SDONSerializable
    public String Color1 = null;
    
    /**
     * The hex RGB color of all even rows.
     */
    @SDONSerializable
    public String Color2 = null;
}

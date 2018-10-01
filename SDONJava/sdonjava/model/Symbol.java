package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * A custom symbol definition from the SmartDraw library.
 * @since SDON 1.2
 */
public class Symbol {
    /**
     * The name of the symbol that can be used for the
     * value of {@link Shape#ShapeType}.
     */
    @SDONSerializable
    public String Name = null;
    
    /**
     * The internal GUID in SmartDraw of the shape
     * specified.
     */
    @SDONSerializable
    public String ID = null;
}

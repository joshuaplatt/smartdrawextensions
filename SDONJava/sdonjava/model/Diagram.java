package sdonjava.model;

import java.util.ArrayList;

import sdonjava.serialization.SDONSerializable;

/**
 * The root object of an SDON document page.
 */
public class Diagram {
    /**
     * The signature of the SDON document.
     * @since SDON 1.2
     */
    @SDONSerializable
    public String Signature = "SmartDrawSDON";
    
    /**
     * The type of driagram SDON will create.  Sets the
     * behavior of lines and shapes.  A value from the
     * DiagramTypes enum.
     */
    @SDONSerializable
    public String DiagramType = null;
    
    /**
     * The version of the SDON document.
     */
    @SDONSerializable
    public double Version = 11;
    
    /**
     * A list of shapes indicating the root shapes (shapes
     * starting without being connected to another shape)
     * for the diagram.
     */
    @SDONSerializable
    public ArrayList<Shape> RootShape = new ArrayList<Shape>();
    
    /**
     * A title string centered over the diagram 1/2" above it.
     */
    @SDONSerializable
    public TitleShape Title = new TitleShape();
    
    /**
     * A list of segmented lines that link shapes together.
     */
    @SDONSerializable
    public ArrayList<Return> Returns = new ArrayList<Return>();
    
    /**
     * A list of custom symbol definitions from the
     * SmartDraw library.  Any symbol defined here can be
     * used in Shape.ShapeType as an addition to the
     * enumerated types.
     * @since SDON 1.2
     */
    @SDONSerializable
    public ArrayList<Symbol> Symbols = new ArrayList<Symbol>();
}
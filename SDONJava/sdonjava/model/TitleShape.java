package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * Represents the title of a diagram.
 */
public class TitleShape {
    /**
     * The text to put as a title for the diagram.
     */
    @SDONSerializable
    public String Label = null;
}
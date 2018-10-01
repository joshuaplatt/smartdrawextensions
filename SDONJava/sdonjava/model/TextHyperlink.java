package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * Represents the container for a hyperlink.
 */
public class TextHyperlink {
    /**
     * The URL of the hyperlink.
     */
    @SDONSerializable
    public String url = null;
}
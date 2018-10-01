package sdonjava.model;

import sdonjava.serialization.SDONSerializable;

/**
 * Container for holding the reference to an image.
 */
public class Image {
    /**
     * The URL of the image.
     */
    @SDONSerializable
    public String url = null;
}
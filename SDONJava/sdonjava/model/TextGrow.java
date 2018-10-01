package sdonjava.model;

/**
 * Indicates how a shape will grow if it holds more text
 * than it can display.
 * @see {@link Shape#TextGrow}
 */
public abstract class TextGrow {
    public static final String Proportional = "Proportional";
    public static final String Vertical = "Vertical";
    public static final String Horizontal = "Horizontal";
}
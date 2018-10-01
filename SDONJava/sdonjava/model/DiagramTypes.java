package sdonjava.model;

/**
 * Type of diagram SDON will create.  Sets the behavior of
 * lines and shapes.
 * @see {@link Diagram#DiagramType}
 */
public abstract class DiagramTypes {
    public static final String Flowchart = "Flowchart";
    public static final String MindMap = "Mindmap";
    public static final String OrgChart = "Orgchart";
    public static final String DecisionTree = "Decisiontree";
    public static final String Hierarchy = "Hierarchy";
    /** @since SDON 1.2 */
    public static final String SQL = "SQL";
}
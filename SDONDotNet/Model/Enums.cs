using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDON.Model
{
    /// <summary>
    /// Type of diagram SDON will create. Sets the behavior of lines and shapes.
    /// </summary>
    public static class SDONTemplates
    {
        public const string Flowchart = "Flowchart";
        public const string MindMap = "Mindmap";
        public const string OrgChart = "Orgchart";
        public const string DecisionTree = "Decisiontree";
        public const string Hierarchy = "Hierarchy";
    }

    /// <summary>
    /// Indicates the horizontal justification of text or shapes.
    /// </summary>
    public static class HorizontalAlignments
    {
        public const string Left = "left";
        public const string Center = "center";
        public const string Right = "right";
    }

    /// <summary>
    /// Indicates the vertical justification of text or shapes.
    /// </summary>
    public static class VerticalAlignments
    {
        public const string Top = "top";
        public const string Middle = "middle";
        public const string Bottom = "bottom";
    }

    /// <summary>
    /// Indicates what type of shape is going to be created by the SDON.
    /// </summary>
    public static class ShapeTypes
    {
        public const string RoundedRectangle = "RRect";
        public const string Oval = "Oval";
        public const string Circle = "Circle";
        public const string Square = "Square";
        public const string Diamond = "Diamond";
    }

    /// <summary>
    /// Indicates the direction of a connector.
    /// </summary>
    public static class Directions
    {
        public const string Left = "Left";
        public const string Right = "Right";
        public const string Top = "Top";
        public const string Bottom = "Bottom";
    }

    /// <summary>
    /// Indiates the pattern of a line.
    /// </summary>
    public static class LinePatterns
    {
        public const string Solid = "Solid";
        public const string Dotted = "Dotted";
        public const string Dashed = "Dashed";
    }

    /// <summary>
    /// Indicates how shapes in a ShapeArray's ShapeArrangement are laid out.
    /// </summary>
    public static class ShapeArrangementTypes
    {
        public const string Square = "Square";
        public const string Row = "Row";
        public const string Column = "Column";
    }

    /// <summary>
    /// Indicates how a shape will grow if it holds more text than it can display.
    /// </summary>
    public static class TextGrow
    {
        public const string Proportional = "Proportional";
        public const string Vertical = "Vertical";
        public const string Horizontal = "Horizontal";
    }

    public static class DataTableTypes
    {
        public const string String = "string";
        public const string Int = "int";
        public const string Float = "float";
        public const string Bool = "bool";
        public const string Date = "date";
    }

    public static class ShapeConnectorTypes
    {
        public const string Flowchart = "Flowchart";
        public const string DecisionTree = "Decisiontree";
        public const string Mindmap = "Mindamp";
        public const string OrgChart = "OrgChart";
    }

    public static class ShapeAlignHorizontal
    {
        public const string Left = "left";
        public const string Right = "right";
        public const string Center = "center";
    }

    public static class ShapeAlignVertical
    {
        public const string Top = "top";
        public const string Bottom = "bottom";
        public const string Middle = "middle";
    }

    public static class ShapeContainerArrangement
    {
        public const string Square = "Square";
        public const string Row = "Row";
        public const string Column = "Column";
    }
}

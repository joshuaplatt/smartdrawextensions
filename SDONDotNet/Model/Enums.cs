using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDON.Model
{
    /// <summary>
    /// Constants table indicating the type of diagram SDON will create. Sets the behavior of lines and shapes.
    /// </summary>
    public static class SDONTemplates
    {
        /// <summary>
        /// The document will open as a flowchart with the flowchart SmartPanel.
        /// </summary>
        public const string Flowchart = "Flowchart";
        /// <summary>
        /// The document will open as a mind map with the mind map SmartPanel.
        /// </summary>
        public const string MindMap = "Mindmap";
        /// <summary>
        /// The document will open as a org chart with the org chart SmartPanel.
        /// </summary>
        public const string OrgChart = "Orgchart";
        /// <summary>
        /// The document will open as a decision tree with the decision tree SmartPanel.
        /// </summary>
        public const string DecisionTree = "Decisiontree";
        /// <summary>
        /// The document will open as a hierarchy diagram with the hierarchy SmartPanel.
        /// </summary>
        public const string Hierarchy = "Hierarchy";
    }

    /// <summary>
    /// Constants table indicating the horizontal justification of text or shapes.
    /// </summary>
    public static class HorizontalAlignments
    {
        /// <summary>
        /// Left-justified alignment.
        /// </summary>
        public const string Left = "left";
        /// <summary>
        /// Center-justified alignment.
        /// </summary>
        public const string Center = "center";
        /// <summary>
        /// Right-justified alignment.
        /// </summary>
        public const string Right = "right";
    }

    /// <summary>
    /// Constants table indicating the vertical justification of text or shapes.
    /// </summary>
    public static class VerticalAlignments
    {
        /// <summary>
        /// Aligns to the top of the shape or container.
        /// </summary>
        public const string Top = "top";
        /// <summary>
        /// Aligns to the middle of the shape or container.
        /// </summary>
        public const string Middle = "middle";
        /// <summary>
        /// Aligns to the bottom of the shape or container.
        /// </summary>
        public const string Bottom = "bottom";
    }

    /// <summary>
    /// Constants table indicating what type of shape is going to be created by the SDON.
    /// </summary>
    public static class ShapeTypes
    {
        /// <summary>
        /// A rectangle with circular edges.
        /// </summary>
        public const string RoundedRectangle = "RRect";
        /// <summary>
        /// An oval.
        /// </summary>
        public const string Oval = "Oval";
        /// <summary>
        /// A circle.
        /// </summary>
        public const string Circle = "Circle";
        /// <summary>
        /// A square.
        /// </summary>
        public const string Square = "Square";
        /// <summary>
        /// A diamond.
        /// </summary>
        public const string Diamond = "Diamond";
    }

    /// <summary>
    /// Constants table indicating the direction of a connector.
    /// </summary>
    public static class Directions
    {
        /// <summary>
        /// The shpes added to a connector will go from left to right.
        /// </summary>
        public const string Left = "Left";
        /// <summary>
        /// The shapes added to a connector will go from right to left.
        /// </summary>
        public const string Right = "Right";
        /// <summary>
        /// The shapes added to a connector will go top to bottom.
        /// </summary>
        public const string Top = "Top";
        /// <summary>
        /// The shapes added to a connector will go bottom to top.
        /// </summary>
        public const string Bottom = "Bottom";
    }

    /// <summary>
    /// Constants table indicating the pattern of a line.
    /// </summary>
    public static class LinePatterns
    {
        /// <summary>
        /// A solid line.
        /// </summary>
        public const string Solid = "Solid";
        /// <summary>
        /// A dotted line.
        /// </summary>
        public const string Dotted = "Dotted";
        /// <summary>
        /// A line made from dashes.
        /// </summary>
        public const string Dashed = "Dashed";
    }

    /// <summary>
    /// Constants table indicating how shapes in a ShapeArray's ShapeArrangement are laid out.
    /// </summary>
    public static class ShapeArrangementTypes
    {
        /// <summary>
        /// Shapes will be arraged in a square grid.
        /// </summary>
        public const string Square = "Square";
        /// <summary>
        /// Shapes will be arranged in a row.
        /// </summary>
        public const string Row = "Row";
        /// <summary>
        /// Shapes will be arranged in a column.
        /// </summary>
        public const string Column = "Column";
    }

    /// <summary>
    /// Constants table indicating how a shape will grow if it holds more text than it can display.
    /// </summary>
    public static class TextGrow
    {
        /// <summary>
        /// Adding text will cause the shape to grow proportionally vertically and horizontally.
        /// </summary>
        public const string Proportional = "Proportional";
        /// <summary>
        /// Adding text will cause the shape to grow vertically only.
        /// </summary>
        public const string Vertical = "Vertical";
        /// <summary>
        /// Adding text will cause the shape to grow horizontally only.
        /// </summary>
        public const string Horizontal = "Horizontal";
    }

    /// <summary>
    /// Constants table indicating the type of data in a data table. Changes how the data behaves.
    /// </summary>
    public static class DataTableTypes
    {
        /// <summary>
        /// The data type is a string of text.
        /// </summary>
        public const string String = "string";
        /// <summary>
        /// The data type is a whole number (an integer).
        /// </summary>
        public const string Int = "int";
        /// <summary>
        /// The data type is a decimal number (a double floating point number).
        /// </summary>
        public const string Float = "float";
        /// <summary>
        /// The data type is a boolean value (either true or false).
        /// </summary>
        public const string Bool = "bool";
        /// <summary>
        /// The data type is a date string (YYYY/MM/DD)
        /// </summary>
        public const string Date = "date";
    }

    /// <summary>
    /// Constants table indicating the type of shape connector to generate. Changes how shapes behave in the diagram.
    /// </summary>
    public static class ShapeConnectorTypes
    {
        /// <summary>
        /// Shapes are added along an evenly-spaced horizontal or vertical shape connector.
        /// </summary>
        public const string Flowchart = "Flowchart";
        /// <summary>
        /// Shapes are added as peers to a parent shape in either a horizontal or vertical arrangement.
        /// </summary>
        public const string DecisionTree = "Decisiontree";
        /// <summary>
        /// Shapes are added as peers to a parent shape in either a horizontal or vertical arrangement.
        /// </summary>
        public const string Mindmap = "Mindamp";
        /// <summary>
        /// Shapes are added as a horizontal or vertical tree stemming from a parent shape.
        /// </summary>
        public const string OrgChart = "OrgChart";
        /// <summary>
        /// Shapes are added as a horizontal tree stemming from a parent shape.
        /// </summary>
        public const string Hierarchy = "Hierarchy";
    }

    /// <summary>
    /// Constants table indicating how shapes are aligned in a shape container horizontally.
    /// </summary>
    public static class ShapeAlignHorizontal
    {
        /// <summary>
        /// Shapes are left-justified in a shape container.
        /// </summary>
        public const string Left = "left";
        /// <summary>
        /// Shapes are right-justified in a shape container.
        /// </summary>
        public const string Right = "right";
        /// <summary>
        /// Shapes are center-justified in a shape container.
        /// </summary>
        public const string Center = "center";
    }

    /// <summary>
    /// Constants table indicating how shapes are aligned in a shape container 
    /// </summary>
    public static class ShapeAlignVertical
    {
        /// <summary>
        /// Shapes are aligned along the top of the shape container.
        /// </summary>
        public const string Top = "top";
        /// <summary>
        /// Shapes are aligned along the bottom of the shape container.
        /// </summary>
        public const string Bottom = "bottom";
        /// <summary>
        /// Shapes are aligned along the middle of the shape container.
        /// </summary>
        public const string Middle = "middle";
    }

    /// <summary>
    /// Constants table indicating the arrangement of shapes in a shape container.
    /// </summary>
    public static class ShapeContainerArrangement
    {
        /// <summary>
        /// Shapes will be aligned in a square grid pattern.
        /// </summary>
        public const string Square = "Square";
        /// <summary>
        /// Shapes are aligned in a row.
        /// </summary>
        public const string Row = "Row";
        /// <summary>
        /// Shapes are aligned in a column.
        /// </summary>
        public const string Column = "Column";
    }

    /// <summary>
    /// Describes the arrangement of shapes on a org chart or hierarchy chart. 
    /// </summary>
    public static class ShapeConnectorArrangement
    {
        /// <summary>
        /// Horizontal row of shapes.
        /// </summary>
        public const string Row = "Row";
        /// <summary>
        /// Horizontal row of shapes with staggered distances from the parent.
        /// </summary>
        public const string Stagger = "Stagger";
        /// <summary>
        /// A verical column of shapes to the right of the connector line.
        /// </summary>
        public const string Column = "Column";
        /// <summary>
        /// A verical column of shapes to the left of the connector line.
        /// </summary>
        public const string LeftColumn = "LeftColumn";
        /// <summary>
        /// A vertical column arranged with shapes on both sides of the connector line.
        /// </summary>
        public const string TwoColumn = "TwoColumn";
    }

    /// <summary>
    /// Constants table indicating icons that can be used in shapes.
    /// </summary>
    public static class Icons
    {
        /// <summary>
        /// A "info" icon.
        /// </summary>
        public const string Info = "Info";
    }

    /// <summary>
    /// Constants table indicating the type of a data value in a data table column.
    /// </summary>
    public static class DataTableDataTypes
    {
        /// <summary>
        /// The data type of a column is a string (text).
        /// </summary>
        public const string String = "String";
        /// <summary>
        /// The data type of a column is an integer (whole number).
        /// </summary>
        public const string Int = "Int";
        /// <summary>
        /// The data type of a column is a floating-point integer (number with a decimal point).
        /// </summary>
        public const string Float = "Float";
        /// <summary>
        /// The data type of a column is a boolean (either true or false).
        /// </summary>
        public const string Bool = "Boolean";
        /// <summary>
        /// The daya type of a column is a date (YYYY/MM/DD).
        /// </summary>
        public const string Date = "Date";
    }

    /// <summary>
    /// Constants table indicating holidays to observe on the Gantt charts.
    /// </summary>
    public static class GanttChartHolidays
    {
        /// <summary>
        /// Observe no holidays.
        /// </summary>
        public const string None = "None";
        /// <summary>
        /// Observe USA holidays.
        /// </summary>
        public const string USA = "USA";
        /// <summary>
        /// Observe United Kingdom holidays.
        /// </summary>
        public const string UK = "UK";
        /// <summary>
        /// Observe Australian holidays.
        /// </summary>
        public const string Australia = "Australia";
        /// <summary>
        /// Observe Canadian holidays.
        /// </summary>
        public const string Canada = "Canada";
    }

    /// <summary>
    /// Special column names for use with Gantt charts. Used when populating the Diagram object's UseDataTable property.
    /// </summary>
    public static class GanttChartColumnNames
    {
        /// <summary>
        /// Numerical index of a row. Defaults to index in the array of rows.
        /// </summary>
        public const string Row = "Row";
        /// <summary>
        /// The description of a task. Defaults to empty.
        /// </summary>
        public const string Task = "Task";
        /// <summary>
        /// The start date of the task in YYYY-MM-DD format. Defaults to the current date.
        /// </summary>
        public const string Start = "Start";
        /// <summary>
        /// The length of the dask in days (can be a decimal). Defaults to 5.
        /// </summary>
        public const string Length = "Length";
        /// <summary>
        /// The end date of the task in YYYY-MM-DD format.
        /// </summary>
        public const string End = "End";
        /// <summary>
        /// The ID of the parent task. Defaults to no parent.
        /// </summary>
        public const string Parent = "Parent";
        /// <summary>
        /// The ID of the task that has to be completed before this task can begin (a dependency).
        /// </summary>
        public const string Master = "Master";
        /// <summary>
        /// The name of the person for the assigned task. Defaults to empty.
        /// </summary>
        public const string Person = "Person";
        /// <summary>
        /// The percent completion of the task. Defaults to 0.
        /// </summary>
        public const string PercentComplete = "PercentComplete";
        /// <summary>
        /// A text field used for indicating which department a task belongs to. Defaults to empty.
        /// </summary>
        public const string Department = "Department";
        /// <summary>
        /// The cost of a task. Defaults to empty.
        /// </summary>
        public const string Cost = "Cost";
        /// <summary>
        /// A custom user text field.
        /// </summary>
        public const string Custom = "Custom";
    }

    /// <summary>
    /// Enum for describing the different arrowheads that can be used.
    /// </summary>
    public enum Arrowheads
    {
        /// <summary>
        /// No arrowhead.
        /// </summary>
        None = 0,
        /// <summary>
        /// Filled Triangle Arrowhead arrowhead.
        /// </summary>
        Filled = 1,	
        /// <summary>
        /// Triangle Arrowhead that is not filled.
        /// </summary>
        LineArrow = 2,	
        /// <summary>
        /// A fancy arrowhead.
        /// </summary>
        Fancy = 3,
        /// <summary>
        /// An arrowhead that is a filled circle.
        /// </summary>
        FilledCircle = 4,
        /// <summary>
        /// An arrowhead that is an empty circle.
        /// </summary>
        EmptyCircle = 5,
        /// <summary>
        /// An arrowhead that is a filled square.
        /// </summary>
        FilledSquare = 6,
        /// <summary>
        /// An arrowhead that is an unfilled square.
        /// </summary>
        EmptySquare = 7,
        /// <summary>
        /// A crow's foot style arrowhead.
        /// </summary>
        CrowsFoot = 8,
        /// <summary>
        /// An arrowhead that is a backslash.
        /// </summary>
        BackSlash = 9,
        /// <summary>
        /// A filled crow's foot style arrowhead.
        /// </summary>
        FilledCrowsFoot = 10,			
        /// <summary>
        /// A filled diamond arrowhead.
        /// </summary>
        Diamond = 11,
        /// <summary>
        /// A zero-to-many relationship arrowhead.
        /// </summary>
        ZeroToMany = 12,
        /// <summary>
        /// A one-to-many relationship arrowhead.
        /// </summary>
        OneToMany = 13,
        /// <summary>
        /// A zero-to-one relationship arrowhead.
        /// </summary>
        ZeroToOne = 14,
        /// <summary>
        /// A one-to-one relationship arrowhead.
        /// </summary>
        OneToOne = 15,
        /// <summary>
        /// A one-to-zero relationship arrowhead.
        /// </summary>
        OneToZero = 16,		
        /// <summary>
        /// A center filled arrowhead.
        /// </summary>
        CenterFilled = 17,		
        /// <summary>
        /// A center line arrowhead.
        /// </summary>
        CenterLineArrow = 18,			
        /// <summary>
        /// A center fancy arrowhead.
        /// </summary>
        CenterFancy = 19,
        /// <summary>
        /// A double arrowhead.
        /// </summary>
        Double = 20,	
        /// <summary>
        /// A filled dimension line arrowhead.
        /// </summary>        
        DimensionFilled = 21,	
        /// <summary>
        /// A plain dimension line arrowhead.
        /// </summary>
        DimensionPlain = 22,
        /// <summary>
        /// The default dimension line arrowhead.
        /// </summary>
        DimensionLine = 23,		
        /// <summary>
        /// The arrowhead is a metafile. (Not supported)
        /// </summary>
        Metafile = 24,
        /// <summary>
        /// An downards arcing arrowhead.
        /// </summary>
        ArcDown = 25,
        /// <summary>
        /// An upwards arcing arrowhead.
        /// </summary>
        ArcUp = 26,
        /// <summary>
        /// A half-arrowhead on top of the line.
        /// </summary>
        HalfUp = 27,
        /// <summary>
        /// A half-arrowhead on the bottom of the line.
        /// </summary>
        HalfDown = 28,
        /// <summary>
        /// A cross shaped arrowhead.
        /// </summary>
        CenterCross = 29,
        /// <summary>
        /// An arrowhead that is a perprendicular line going up out of the line.
        /// </summary>
        HalfLineUp = 30,
        /// <summary>
        /// An arrowhead that is a perprendicular line going down out of the line.
        /// </summary>
        HalfLineDown = 31,	
        /// <summary>
        /// A forward slash arrowhead.
        /// </summary>
        ForwardSlash = 32,
        /// <summary>
        /// An open arrowhead that is filled.
        /// </summary>
        OpenFilled = 33,
        /// <summary>
        /// An open crow's foot arrowhead.
        /// </summary>
        OpenCrowsFoot = 34,		
        /// <summary>
        /// An open diamond arrowhead.
        /// </summary>
        OpenDiamond = 35,		
        /// <summary>
        /// A cross shaped arrowhead.
        /// </summary>
        Cross = 36,	
        /// <summary>
        /// An indicator arrowhead on the bottom of the line.
        /// </summary>
        IndicatorDown = 37,
        /// <summary>
        /// An indicator arrowhead on the top of the line.
        /// </summary>
        IndicatorUp = 38,
        /// <summary>
        /// A filled circular end.
        /// </summary>
        RoundEnd = 39,
    }
}

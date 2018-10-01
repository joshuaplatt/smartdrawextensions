using SDON.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SDON.Model
{
    /// <summary>
    /// Object for containing information about a column in a gantt chart.
    /// </summary>
    [Serializable]
    public sealed class GanttColumn : SDONSerializeable
    {
        [DataMember(Name = "Name")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _name = null;

        [DataMember(Name = "Settings")]
        [IgnoreIfDefaultValue(Default = null)]
        private GanttColumnSettings _settings = null;

        /// <summary>
        /// The name of the column. Must be a value from GanttChartColumnNames.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The settings of the column.
        /// </summary>
        public GanttColumnSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }
    }
}

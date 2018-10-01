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
    /// Object for containing the configuration options for a Gantt chart.
    /// </summary>
    [Serializable]
    public sealed class GanttOptions : SDONSerializeable
    {
        [DataMember(Name = "AllWorkingDays")]
        [IgnoreIfDefaultValue(Default = null)]
        private bool? _allWorkingDays = null;

        [DataMember(Name = "Holidays")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _holidays = null;

        /// <summary>
        /// Whether or not Saturday and Sunday are considered working days or if they should be omitted from the Gantt chart.
        /// </summary>
        public bool AllWorkingDays
        {
            get
            {
                if (_allWorkingDays == null) return false;
                return _allWorkingDays.Value;
            }
            set
            {
                _allWorkingDays = value;
            }            
        }

        /// <summary>
        /// The holidays (by region) to include in the Gantt chart. Must be a value from GanttChartHolidays enum.
        /// </summary>
        public string Holidays
        {
            get { return _holidays; }
            set { _holidays = value; }
        }
    }
}

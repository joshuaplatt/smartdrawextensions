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
    /// Object for containing various settings for a column in a gantt chart.
    /// </summary>
    [Serializable]
    public sealed class GanttColumnSettings : SDONSerializeable
    {
        [DataMember(Name = "Title")]
        [IgnoreIfDefaultValue(Default = null)]
        private string _title = null;

        [DataMember(Name = "Width")]
        [IgnoreIfDefaultValue(Default = -1)]
        private int _width = -1;

        /// <summary>
        /// The title of the column in the gantt chart.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// The width of the column.
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
    }
}

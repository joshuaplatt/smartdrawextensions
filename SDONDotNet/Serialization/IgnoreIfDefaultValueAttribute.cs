using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace SDON.Serialization
{
    /// <summary>
    /// Attribute for telling a serializer not to serialze a property if its value matches the default value for the field or property.
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class IgnoreIfDefaultValueAttribute : Attribute
    {
        protected object _default = null;

        /// <summary>
        /// The default value to compare the current value to.
        /// </summary>
        public object Default
        {
            get { return _default; }
            set { _default = value; }
        }
    }
}

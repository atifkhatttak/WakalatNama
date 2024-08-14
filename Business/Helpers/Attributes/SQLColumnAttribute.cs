using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers.Attributes
{
     
    [System.AttributeUsage(System.AttributeTargets.All)]

    public class SQLColumnAttribute : System.Attribute
    {
        /// <summary>
        /// This field will be used for SQL Mapping field 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This field while set to true, will apply like query for string
        /// </summary>
        public bool IsContain { set; get; }
        /// <summary>
        /// This field while set to true, will apply equal query for Date
        /// </summary>
        public bool IsDataEqual { get; set; }
        /// <summary>
        /// This field while set to true will apply Less Than or Equal query for Date
        /// </summary>
        public bool IsDateLessOrEqual { get; set; }
        /// <summary>
        /// This field while set to true, will apply >= query for Date
        /// </summary>
        public bool IsDateGreaterOrEqual { get; set; }
    }
}

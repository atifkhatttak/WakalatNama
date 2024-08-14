using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public abstract class SQLBasedVm
    {
        [JsonIgnore]
        public  abstract string? ReportType { get; } 
    }
}

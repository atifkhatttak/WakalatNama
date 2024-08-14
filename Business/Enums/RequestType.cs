using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{
    public enum RequestType
    {
        [Description("Use for case and case date")]
        CourtCase =1,
        [Description("Use for uploading profile documents")]
        Profile = 2,
        [Description("Use for chat documents")]
        Chat = 3,
        [Description("Use for user documents")]
        User = 4
    }
}

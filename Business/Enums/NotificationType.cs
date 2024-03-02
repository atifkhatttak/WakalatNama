using Business.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{
    public enum NotificationType :int
    {
        [Message("Hi, {FullName} Welcome to WakalatNaama")]
        [Description("Greeting")]
        Greeting =1,
        [Message("ForgotPassword")]
        [Description("ForgotPassword")]
        ForgotPassword,
    }
}

using Business.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{
    public enum NotificationType
    {
        None=-1,
        [Message("Hi, {FullName} Welcome to WakalatNaama")]
        [Description("Greeting")]
        Greeting =1,
        [Message("ForgotPassword")]
        [Description("ForgotPassword")]
        ForgotPassword=2,
        [Message("Notification for Case No. {CaseTitle}: The next hearing is scheduled for {Date} at {CourtName}")] 
        [Description("Case Date")]
        CaseDate=3,
        [Message("")]
        [Description("Case Notification")]
        CaseNotification = 4,
        [Message("Your account has been approved")]
        [Description("Account Approval")]
        AccountApproval = 5,
        [Message("Your dairy notification")]
        [Description("Dairy Notification")]
        DairyNotification = 6,
        [Message("You received new message From {SenderName}")]
        [Description("Chat Notification")]
        ChatNotification = 7
    }
}

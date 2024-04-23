using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{
  public enum  CaseStatuses
    {
        [Description("Case Created")]
        New = 1,
        [Description("Accepted by Admin")]
        AdminAccepted = 2,
        [Description("Rejected by Admin")]
        AdminRejected = 3,
        [Description("Approved by Admin and Awaiting Lawyer Response")]
        ForwardedToLawyer=4,
        [Description("Accepted by Lawyer")]
        LawyerAccepted = 5,
        [Description("Rejected by Lawyer")]
        LawyerRejected = 6,
        [Description("Approved")]
        Approved=7
    }
}

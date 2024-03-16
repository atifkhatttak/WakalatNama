using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{
  public enum  CaseStatuses
    {
        New=1,
        AdminAccepted=2,
        AdminRejected=3,
        LawyerAccepted=4,
        LawyerRejected=5,
    }
}

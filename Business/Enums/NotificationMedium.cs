using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{
    public enum NotificationMedium: int
    {
        SMS=1,
        Email,
        Both
    }
}

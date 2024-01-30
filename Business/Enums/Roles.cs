using Business.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{
    public enum Roles
    {
        [Message("Please add admin")]
        [Description("Admin")]
        Admin,
        [Message("Please add zonal manager")]
        [Description("Zonal Manager")]
        Zonal_Manager,
        [Message("Please add citizen")]
        [Description("Citizen")]
        Citizen,
        [Message("Please add lawyer")]
        [Description("Laywer")]
        Laywer,
        [Message("Please add employee")]
        [Description("Employee")]
        Employee

    }
}

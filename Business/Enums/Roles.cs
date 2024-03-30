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
        Admin=1,
        [Message("Please add zonal manager")]
        [Description("Zonal Manager")]
        Zonal_Manager=2,
        [Message("Please add citizen")]
        [Description("Citizen")]
        Citizen=3,
        [Message("Please add Lawyer")]
        [Description("Lawyer")]
        Lawyer=4,
        [Message("Please add employee")]
        [Description("Employee")]
        Employee=5

    }
    public enum ECasePlacingType
    {
        [Description("OnDemand Case, citizen select a lawyer")]
        OnDemand = 1,
        [Description("Free legal assistance providing to citizen")]
        FreeAssistance = 2,
        [Description("Lets platform decide better lawyer for citizen")]
        PlatformDecide = 3   
    }
    public enum EDocumentType
    {
        None = 0,
        NicFront = 1,
        NicBack = 2,
        ProfilePice = 3,
        NICOPFront = 4,
        NICOPFback = 5,
        EducationPic = 6,
        CertificationPic = 7,
        PassportPicFront = 8,
        PassportPicBack = 9,
        BarCouncilCardScanFront = 10,
        BarCouncilCardScanBack = 11,
        CitizenDownloadable=12,
        LawyerDownloadable=13,
        CaseDocument=14,
        CaseDateDocument=15

    }
    public enum CaseLegalStatus
    {
        Initiated=1, 
            Draft=2,
            Completed=3
    }
}

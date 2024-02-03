using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utilities
{
    public class AppEnums
    {

        public enum DocumentType
        {
            None = 0,
            NicFront = 1, 
            NicBack = 2,
            ProfilePice = 3,
            NICOPFront = 4, 
            NICOPFback = 5,
            EducationPic = 6,
            CertificationPic = 7,
            PassportPicFront= 8,
            PassportPicBack= 9,
            BarCouncilCardScanFront= 10,
            BarCouncilCardScanBack= 11
        }
    }
}

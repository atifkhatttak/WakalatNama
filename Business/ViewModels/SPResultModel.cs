using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class sp_GetCitizenLawyers_Result
    {
        public long? UserId { get; set; }
        public string? FullName { get; set; }
        public float? TotalExperience { get; set; }
        public float? Rating { get; set; }
        public bool? IsFavourite { get; set; }
    }

}

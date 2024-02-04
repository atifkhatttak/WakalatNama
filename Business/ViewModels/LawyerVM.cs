using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class LawyerVM
    {
        public LawyerVM() { }
        public long? Id { get; set; }
        public string? UserName { get; set; }
        public float? TotalExperience { get; set; }
        public float? Rating { get; set; }
        public string? ProfilePic { get; set;}
        public bool? IsFavourite { get; set;}
        public string ProfileDescription { get; set;}
        public int? CompletedCase { get; set; }
        public int? TotalClient { get; set; }
    }
}

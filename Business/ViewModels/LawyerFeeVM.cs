using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Business.ViewModels
{
    public class LawyerFeeStructureVM
    {
        public int? FeeId { get; set; }
        [Required(ErrorMessage = "LawyerFee is required")]
        [Range(0,1000000,ErrorMessage ="LawyerFee should be between 0 and 10 lac")]
        //[Precision(18,3)]
        public decimal LawyerFee { get; set; }
        [Required(ErrorMessage = "Minimum experience is required")]
        public int ExpMin { get; set; }
        [Required(ErrorMessage = "Maximum experience is required")]
        public int ExpMax { get; set; }
        [Required(ErrorMessage = "Case nature is required")]
        public int CaseNatureId { get; set; }
        [Required(ErrorMessage = "Jurisdiction is required")]
        public int JurisdictionId { get; set; }
        [DefaultValue(false)]
        public bool IsForeignQualified { get; set; }

    }
    public class GetLawyerFeeVM
    {
        [Required(ErrorMessage = "Minimum experience is required")]
        //[Range(1,100,ErrorMessage ="Minimum xperience between 1 to 100")]
        public int ExpMin { get; set; }
        [Required(ErrorMessage = "Maximum experience is required")]
        //[Range(1, 100, ErrorMessage = "Maximum xperience between 1 to 100")]
        public int ExpMax { get; set; }
        [Required(ErrorMessage = "Case nature is required")]
        public int CaseNatureId { get; set; }
        [Required(ErrorMessage = "Jurisdiction is required")]
        public int JurisdictionId { get; set; }
    }
}

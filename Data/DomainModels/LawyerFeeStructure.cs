using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class LawyerFeeStructure :BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeeId { get; set; }
        //[Display(Name = "2-5 Years Experience")]
        //[Range(0.01, 9999999.99, ErrorMessage = "Value must be between 0.01 and 9999999.99")]
        public decimal LawyerFee { get; set; }
        public int ExpMin { get; set; }
        public int ExpMax { get; set; }
        public int CaseNatureId { get; set; }
        public int JurisdictionId { get; set; }
        [DefaultValue(false)]
        public bool IsForeignQualified { get; set; }

    }
}

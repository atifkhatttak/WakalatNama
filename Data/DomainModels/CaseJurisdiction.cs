using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Data.DomainModels;

namespace ProjWakalatnama.DataLayer.Models
{
    public class CaseJurisdiction : BaseModel
    {
        [Key]
        public int CaseJurisdictionId { get; set; }

        [Required]
        [StringLength(100)]
        public string JurisdictionName { get; set; }

        [DefaultValue(false)]
        public bool? IsDeleted { get; set; }
    }
}

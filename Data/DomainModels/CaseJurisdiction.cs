using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjWakalatnama.DataLayer.Models
{
    public class CaseJurisdiction
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

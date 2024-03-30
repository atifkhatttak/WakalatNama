using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Data.DomainModels;

namespace ProjWakalatnama.DataLayer.Models
{
    public class CasesDetail : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public long CaseId { get; set; }

        [Required]
        public int CaseStatusId { get; set; }
        public string? CaseDateTitle { get; set; }

        [MaxLength]
        public string DateDescription { get; set; }
        public DateTime? HearingDate { get; set; }

    }
}

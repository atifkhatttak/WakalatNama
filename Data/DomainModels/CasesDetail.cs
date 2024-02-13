using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjWakalatnama.DataLayer.Models
{
    public class CasesDetail
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

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long? CreatedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedUser { get; set; }

        [DefaultValue(false)]
        public bool? IsDeleted { get; set; }
    }
}

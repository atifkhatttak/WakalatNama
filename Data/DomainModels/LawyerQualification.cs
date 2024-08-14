using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class LawyerQualification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(100)]
        public string? DegreeName { get; set; }
        [StringLength(100)]
        public string? InstituteName { get; set; }
        [Required]
        public long UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class CasesDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DocumentId { get; set; }

        [Required]
        public long CaseId { get; set; }
        [StringLength(250)]
        public string? DocName { get; set; }
        [StringLength(250)]
        public string? DocPath { get; set; }
        public string? DocExtension { get; set; }
        public int? DocTypeId { get; set; }
        public bool? IsUploaded { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedUser { get; set; }

        [DefaultValue(false)]
        public bool? IsDeleted { get; set; }
    }
}

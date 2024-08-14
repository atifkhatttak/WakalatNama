using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class Payment:BaseModel
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PaymentId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? RemainingAmount { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public int PaymentMethodId { get; set; }
        [StringLength(255)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Purpose { get; set; }
        [Required]
        public long FromUserId { get; set; }
        [Required]
        public long ToUserId { get; set; }
        public long? CaseId { get; set; }
        public long? CaseDateId { get; set; }
        [StringLength(20)]
        public string? FromAccount { get; set; }
        [StringLength(20)]
        public string? ToAccount { get; set; }
        public string? RecieptUrl { get; set; }

    }
}

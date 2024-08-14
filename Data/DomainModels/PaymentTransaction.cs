using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class PaymentTransaction:BaseModel
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TransactionId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public int PaymentMethod { get; set; }
        [StringLength(255)]
        public string? TransactionDescription { get; set; }
        [StringLength(100)]
        public string? Purpose { get; set; }
        public string? GatewayResponse { get; set; }
        [Required]
        public long PaymentId { get; set; }
        [Required]
        public long FromUserId { get; set; }
        [Required]
        public long ToUserId { get; set; }
        public long? CaseId { get; set; }
        public long? CaseDateId { get; set; }
        public string? FromAccount { get; set; }
        public string? ToAccount { get; set; }
        public string? RecieptUrl { get; set; }

    }
}

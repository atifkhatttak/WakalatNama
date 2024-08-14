using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class PaymentVM
    {
        public long? PaymentId { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public long? FromUserId { get; set; }
        public long? ToUserId { get; set; }
        public string? PaymentUrl { get; set; }
        public int PaymentMethod { get; set; }
    }

    public class LawyerCasePaymentsVM
    {
        public long PaymentId { get;  set; }
        public decimal? Amount { get;  set; }
        public decimal? RemainingAmount { get; set; }
        public DateTime? AcceptanceDate { get;  set; }
        public long CaseId { get;  set; }
        public string? CaseTitle { get;  set; }
    }
    public class LawyerCaseTransactionVM
    {
        public long PaymentId { get; set; }
        public long TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long CaseId { get; set; }
        public long CaseDateId { get; set; }
    }
}

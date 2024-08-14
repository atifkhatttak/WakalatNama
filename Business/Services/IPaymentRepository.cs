using Business.BusinessLogic;
using Business.ViewModels;
using Data.DomainModels;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<bool> PostPayment(PaymentVM vm);
        Task<List<LawyerCasePaymentsVM>> GetLawyerPayment(long UserId);
        Task<List<LawyerCaseTransactionVM>> GetLawyerPaymentByCase(long UserId,long CaseId);

    }
}

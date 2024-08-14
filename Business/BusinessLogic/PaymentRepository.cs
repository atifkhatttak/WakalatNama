using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        //private readonly string LoggedInRole = "";

        public PaymentRepository(WKNNAMADBCtx ctx, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
            this._httpContextAccessor = httpContextAccessor;

            this.isAuthenticated = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
            //this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("role")) : -1;
        }

        public async Task<List<LawyerCasePaymentsVM>> GetLawyerPayment(long UserId)
        {
            return await (from p in ctx.Payments
             join cc in ctx.CourtCases on p.CaseId equals cc.CaseId
             where cc.LawyerId == UserId || cc.RedundantLawyerId == UserId && !p.IsDeleted && !cc.IsDeleted
                          select new LawyerCasePaymentsVM
             {
                PaymentId= p.PaymentId,
                 Amount= p.Amount,
                 RemainingAmount= p.RemainingAmount,
                 AcceptanceDate=cc.AcceptanceDate,
                 CaseId= cc.CaseId,
                 CaseTitle= cc.CaseTitle
             })
             .ToListAsync();
        }

        public async Task<List<LawyerCaseTransactionVM>> GetLawyerPaymentByCase(long UserId, long CaseId)
        {
            return await (from p in ctx.Payments
                         join pt in ctx.PaymentTransactions on p.PaymentId equals pt.PaymentId
                         join cc in ctx.CourtCases on pt.CaseId equals cc.CaseId
                         where cc.LawyerId == UserId || cc.RedundantLawyerId == UserId && cc.CaseId==CaseId && !p.IsDeleted && !pt.IsDeleted && !cc.IsDeleted
                         select new LawyerCaseTransactionVM
                         {
                             PaymentId = p.PaymentId,
                             Amount = p.Amount,
                             TransactionDate=pt.TransactionDate,
                             TransactionId=pt.TransactionId,
                             CaseId = cc.CaseId,
                             CaseDateId=0
                         })
            .ToListAsync();

        }

        public async Task<bool> PostPayment(PaymentVM vm)
        {
            int res=0;
            try
            {
                if (vm!=null)
                {
                    var citizen = await ctx.UserProfiles.FirstOrDefaultAsync(x=>x.UserId==vm.FromUserId && x.IsActive==true && !x.IsDeleted);
                    ctx.Payments.Add(new Payment()
                    {
                        Amount=vm.PaidAmount,
                        FromUserId=vm.FromUserId??-1,
                        ToUserId=(citizen?.RoleId==(int)Roles.Citizen)?-1:vm.ToUserId??-1,
                        RecieptUrl=vm.PaymentUrl,
                        PaymentMethodId=vm.PaymentMethod,
                        PaymentDate = DateTime.Now
                    });

                 res=   await ctx.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return res!=0;
        }
    }
}

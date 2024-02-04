using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class CasesRepository : BaseRepository<CourtCases>, ICasesRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;

        public CasesRepository(WKNNAMADBCtx ctx, IConfiguration config) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
        }

        public async Task CreateCase(CourtCaseVM courtCase)
        {
            try
            {
                if (courtCase!=null)
                {

                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}

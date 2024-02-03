using Business.Services;
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
    public class UserRepository : BaseRepository<UserProfile>, IUserRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;

        public UserRepository(WKNNAMADBCtx ctx, IConfiguration config) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
        }
    }
}

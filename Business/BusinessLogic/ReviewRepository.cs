using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;

        public ReviewRepository(WKNNAMADBCtx ctx, IConfiguration config) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
        }
        public async Task<List<Review>> GetUserReviews(int? userId)
        {
            List<Review> reviews = new List<Review>();
            try
            {
                if (userId != null && userId > 0)
                {
                    reviews = await ctx
                        .Reviews
                        .Where(x => x.CommitOnId == userId && x.IsDeleted == false)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reviews;
        }
    }
}

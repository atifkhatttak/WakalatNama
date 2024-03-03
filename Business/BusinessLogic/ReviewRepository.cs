using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        //private readonly string LoggedInRole = "";

        public ReviewRepository(WKNNAMADBCtx ctx, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
            //this._httpContextAccessor = httpContextAccessor;

            this.isAuthenticated = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
            //this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("role")) : -1;
        }

        public async Task<List<LawyerVM>> GetFavouriteLawyers(long? userId)
        {
            List<LawyerVM> lawyerList = new List<LawyerVM>();
            try
            {

                if (userId != null)
                {
                    var d = await (from u in ctx.UserProfiles
                                   join f in ctx.Favourites on u.UserId equals f.UserId
                                   where f.IsDeleted == false
                                   && u.IsActive == true && u.IsVerified == true && u.IsDeleted == false
                                   select new
                                   {
                                       u.UserId,
                                       f.LawyerId,
                                       u.FullName,
                                       u.TotalExperience,
                                       u.Rating,
                                       f.IsFavourite
                                   }
                            ).ToListAsync();
                    //await ctx
                    //.UserProfiles
                    //.Where(x => x.UserId == userId && x.RoleId == (int)Roles.Lawyer && x.IsActive == true && x.IsVerified == true && x.IsDeleted == false)
                    //.ToListAsync();

                    if (d != null)
                    {
                        foreach (var item in d)
                        {
                            lawyerList.Add(new LawyerVM
                            {
                                CitizenId = item.UserId,
                                LawyerId=item.LawyerId,
                                UserName = item.FullName,
                                TotalExperience = item.TotalExperience,
                                Rating = item.Rating,
                                IsFavourite = item.IsFavourite
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyerList;
        }

        public async Task<List<Review>> GetUserReviews(int? userId)
        {
            List<Review> reviews = new List<Review>();
            try
            {
                //if (!isAuthenticated) return userCases;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId) 
                //        return userCases;
                //}

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

        public async Task MarkFaourite(ReviewFavouriteVM favouriteVM)
        {
            try
            {
                //if (!isAuthenticated) return;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != favouriteVM.UserId)
                //        return;
                //}

                if (favouriteVM!=null && favouriteVM.UserId>0)
                {
                   await ctx.AddAsync(new Favourite()
                    {
                        UserId =favouriteVM.UserId,
                        LawyerId = favouriteVM.LawyerId,
                        IsFavourite = favouriteVM.IsFavourite
                    });
                   await ctx.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

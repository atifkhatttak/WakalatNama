using Business.Enums;
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
    public class UserRepository : BaseRepository<UserProfile>, IUserRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;

        public UserRepository(WKNNAMADBCtx ctx, IConfiguration config) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
        }

        public async Task<List<LawyerVM>> GetLawyerList(FilterVM filterVM)
        {
            List<LawyerVM> lawyerList = new List<LawyerVM>();
            try
            {

                if (filterVM != null)
                {
                    var d = await ctx
                        .UserProfiles
                        .Where(x => x.CityId == filterVM.CityId || (x.TotalExperience >= filterVM.ExperienceMin && x.TotalExperience <= filterVM.ExperienceMax)
                         && (x.RoleId == (int)Roles.Laywer && x.IsActive == true && x.IsVerified == true && x.IsDeleted == false))
                        .ToListAsync();

                    if (d != null)
                    {
                        foreach (var item in d)
                        {
                            lawyerList.Add(new LawyerVM
                            {
                                Id = item.UserId,
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
        public async Task<LawyerVM> GetLawyerProfile(long? LawyerId)
        {
            LawyerVM lawyer = new LawyerVM();
            try
            {

                if (LawyerId > 0)
                {
                    var d = await ctx.UserProfiles.Where(x => x.UserId== LawyerId && x.RoleId==(int)Roles.Laywer).FirstOrDefaultAsync();
                    int CasesCount=0;
                    int TotalClient = 0;
                    if (d != null)
                    {
                        lawyer.Id = d.UserId;
                        lawyer.UserName = d.FullName;
                        lawyer.ProfilePic = "";
                        lawyer.ProfileDescription = d.ProfileDescription!;
                        lawyer.TotalExperience = d.TotalExperience;
                        lawyer.Rating = d.Rating;
                        lawyer.IsFavourite = d.IsFavourite;
                        lawyer.CompletedCase = CasesCount;
                        lawyer.TotalClient = TotalClient;
                    }
                }              
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyer;
        }
        public async Task<LawyerHomeVM> GetLawyerHome(int? lawyerId)
        {
            LawyerHomeVM lawyer = new LawyerHomeVM();
            try
            {

                if (lawyerId > 0)
                {
                    lawyer.TotalCases = await ctx.CourtCases.CountAsync(x => x.LawyerId== lawyerId && x.IsDeleted==false);
                    lawyer.CompltedCase = await ctx.CourtCases.CountAsync(x => x.LawyerId== lawyerId && x.CaseStatusId==(int)CaseStatus.Initiated && x.IsDeleted==false);
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyer;
        }
        public async Task<CitizenVM> GetCitizenProfile(long? CitizenId)
        {
            CitizenVM citizenVM= new CitizenVM();
            try
            {

                if (CitizenId > 0)
                {
                    var d = await ctx
                        .UserProfiles
                        .Where(x => x.UserId == CitizenId
                    && (x.RoleId == (int)Roles.Laywer && x.IsActive == true && x.IsVerified == true && x.IsDeleted == false))
                        .FirstOrDefaultAsync();

                    if (d != null)
                    {
                        citizenVM.UserId = d.UserId;
                        citizenVM.ProfileId = d.ProfileId;
                        citizenVM.ProfilePic = "";
                        citizenVM.FullName = d.FullName;
                        citizenVM.FatherName = d.FatherName;
                        citizenVM.Email = d.Email;
                        citizenVM.CNICNo = d.CNICNo;
                        citizenVM.ContactNumber = d.ContactNumber;
                        citizenVM.CurrAddress = d.CurrAddress;
                        citizenVM.PermAddress = d.PermAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return citizenVM;
        }
    }
}

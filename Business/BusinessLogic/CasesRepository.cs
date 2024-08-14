using Business.Chat_Hub;
using Business.Enums;
using Business.Helpers;
using Business.Helpers.Attributes;
using Business.Helpers.Extension;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Business.BusinessLogic
{
    public class CasesRepository : BaseRepository<CasesDetail>, ICasesRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageRepository messageRepository;
        private readonly IDocumentServiceV2 documentService;
        private readonly INotificationRepository _notification;
        private readonly ILogger<CasesRepository> _logger;
        private readonly IEmailService _emailService;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        private readonly string LoggedInRole = "";
        private readonly ChatHub chatHub;

        public CasesRepository(WKNNAMADBCtx ctx, IConfiguration config, IHttpContextAccessor httpContextAccessor, ILogger<CasesRepository> logger, 
            IDocumentServiceV2 documentService, IMessageRepository messageRepository, ChatHub chatHub, INotificationRepository notification, IEmailService emailService) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
            this._httpContextAccessor = httpContextAccessor;
            _logger = logger;
            this.isAuthenticated = _httpContextAccessor?.HttpContext?.User.Identity.IsAuthenticated ?? false;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
            this.documentService = documentService;
            this.messageRepository = messageRepository;
            this.chatHub = chatHub;
            _notification = notification;
            this._emailService = emailService;
            //this.LoggedInRole = isAuthenticated ? _httpContextAccessor.HttpContext.User.FindFirstValue("role").ToString() : "";
        }

        public async Task<CourtCases> CreateUpdateCase(CourtCaseVM caseVM)
        {
            CourtCases courtCases = new CourtCases();
            try
            {
                //if (!isAuthenticated) return;

                if (caseVM != null)
                {
                    if (caseVM?.CaseId > 0)
                    {
                        var CheckCaseExist = await ctx.CourtCases.Where(x => x.CaseId == caseVM.CaseId && !x.IsDeleted).FirstOrDefaultAsync();
                        if (CheckCaseExist != null)
                        {
                            CheckCaseExist.LawyerId = caseVM.LawyerId;
                            CheckCaseExist.RedundantLawyerId = caseVM.RedundantLawyerId;
                            CheckCaseExist.CaseTitle = caseVM.CaseTitle;
                            CheckCaseExist.PartyId = caseVM.PartyId;
                            CheckCaseExist.CategoryId = caseVM.CategoryId;
                            CheckCaseExist.CaseDescription = caseVM.CaseDescription!;
                            CheckCaseExist.CaseJurisdictionId = caseVM.CaseJurisdictionId;
                            CheckCaseExist.CourtId = caseVM.CourtId;
                            CheckCaseExist.CasePlacingId = caseVM.CasePlacingId;
                            CheckCaseExist.StatusId = (int)ECaseStatuses.New;
                            CheckCaseExist.CasePlacedFor = caseVM.CasePlacedFor;

                            ctx.Entry(CheckCaseExist).State = EntityState.Modified;
                            await ctx.SaveChangesAsync();

                            courtCases = CheckCaseExist;
                        }
                    }
                    else
                    {
                        courtCases.CitizenId = caseVM.CitizenId;
                        courtCases.LawyerId = caseVM.LawyerId;
                        courtCases.RedundantLawyerId = caseVM.RedundantLawyerId;
                        courtCases.CaseNumber = caseVM.CaseNumber ?? "";
                        courtCases.CaseTitle = caseVM.CaseTitle;
                        courtCases.PartyId = caseVM.PartyId;
                        courtCases.CategoryId = caseVM.CategoryId;
                        courtCases.CaseDescription = caseVM.CaseDescription!;
                        courtCases.CaseJurisdictionId = caseVM.CaseJurisdictionId;
                        courtCases.CourtId = caseVM.CourtId;
                        courtCases.CasePlacingId = caseVM.CasePlacingId;
                        courtCases.LegalStatusId = (int)ELeagalStatus.None;
                        courtCases.StatusId = (int)ECaseStatuses.New;
                        courtCases.CasePlacedFor = caseVM.CasePlacedFor;

                        await ctx.AddAsync(courtCases);
                        await ctx.SaveChangesAsync();

                        #region Case Notification

                        try
                        {                       
                        //send email
                        var getUser = await ctx.UserProfiles.FirstOrDefaultAsync(x => x.UserId == courtCases.CitizenId && !x.IsDeleted);
                        string body= $"{getUser?.FullName} placed new case";
                        await _emailService.SendMailTrapEmail($"{courtCases?.CaseTitle}",body, getUser?.Email);
                       
                        //Send Notification
                        var getAdmin = await ctx.UserProfiles.FirstOrDefaultAsync(x => x.RoleId == (int)Roles.Admin);
                        NotificationVm vm = new NotificationVm();
                        vm.NotificationType = (int)NotificationType.CaseDate;
                        vm.ToUserId = getAdmin.UserId;
                        vm.FromUserId = courtCases.CitizenId;
                        vm.Content = body;

                        await _notification.BroadCastAndSaveNotification(vm);
                        }
                        catch (Exception)
                        {
                        }
                        #endregion

                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return courtCases;
        }

        public async Task<CourtCaseVM> GetCaseById(long? caseId)
        {
            CourtCaseVM? courtCase = new CourtCaseVM();
            try
            {
                //if (!isAuthenticated) return courtCase;

                if (caseId != null)
                {
                    courtCase = await (from c in ctx.CourtCases
                                       join u in ctx.UserProfiles on c.CitizenId equals u.UserId
                                       join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                       where c.CaseId == caseId && !u.IsDeleted && !c.IsDeleted
                                       select new CourtCaseVM
                                       {
                                           UserFullName = u.FullName,
                                           CaseId = c.CaseId,
                                           CitizenId = c.CitizenId,
                                           LawyerId = c.LawyerId,
                                           CaseNumber = c.CaseNumber,
                                           CaseTitle = c.CaseTitle,
                                           CaseDescription = c.CaseDescription,
                                           CategoryId = c.CategoryId,
                                           CategoryName = cat.CategoryName,
                                           CaseStatusId = c.LegalStatusId,
                                           CasePlacingId = c.CasePlacingId
                                       })
                          .FirstOrDefaultAsync();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return courtCase;
        }
        public async Task<CourtCaseDetailsVM> GetCaseDetailsById(long? caseId)
        {
            CourtCaseDetailsVM? courtCase = new CourtCaseDetailsVM();
            try
            {
                //if (!isAuthenticated) return courtCase;

                if (caseId != null)
                {
                    courtCase = await (from c in ctx.CourtCases
                                       join u in ctx.UserProfiles on c.LawyerId equals u.UserId
                                       join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                       join pt in ctx.PartyStatuses on c.PartyId equals pt.ID
                                       join jd in ctx.CaseJurisdictions on c.CaseJurisdictionId equals jd.CaseJurisdictionId
                                       where c.CaseId == caseId && !u.IsDeleted && !c.IsDeleted
                                       select new CourtCaseDetailsVM
                                       {
                                           UserFullName = u.FullName,
                                           CaseId = c.CaseId,
                                           CitizenId = c.CitizenId,
                                           LawyerId = c.LawyerId,
                                           CaseNumber = c.CaseNumber,
                                           CaseTitle = c.CaseTitle,
                                           CaseDescription = c.CaseDescription,
                                           CategoryId = c.CategoryId,
                                           CategoryName = cat.CategoryName,
                                           CaseStatusId = c.LegalStatusId,
                                           CasePlacingId = c.CasePlacingId,
                                           PartStatus=pt.StatusName,
                                           CaseJurisdiction=jd.JurisdictionName,
                                           CourtLocation="Islamabad",
                                           CaseFee=0
                                       })
                          .FirstOrDefaultAsync();

                    courtCase.CaseFiles = await documentService.FilesCollections(new FileParmsVM() { CaseId = courtCase.CaseId,DocType = (int)EDocumentType.CaseDocument });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return courtCase;
        }

        public async Task<List<CourtCaseVM>> GetCitizenCases(long? userId)
        {
            List<CourtCaseVM> userCases = new List<CourtCaseVM>();
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
                    var caseList = await (from u in ctx.UserProfiles
                                          join c in ctx.CourtCases on u.UserId equals c.CitizenId
                                          join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                          where c.CitizenId == userId && u.RoleId == (int)Roles.Citizen && !(u.IsDeleted && c.IsDeleted)
                                          select new CourtCaseVM
                                          {
                                              UserId = userId,
                                              UserFullName = u.FullName,
                                              CaseId = c.CaseId,
                                              CitizenId = c.CitizenId,
                                              LawyerId = c.LawyerId,
                                              CaseNumber = c.CaseNumber,
                                              CaseTitle = c.CaseTitle,
                                              CategoryId = c.CategoryId,
                                              CategoryName = cat.CategoryName,
                                              CasePlacingId = c.CasePlacingId,
                                              
                                          })
                      .ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            userCases.Add(new CourtCaseVM
                            {
                                UserId = userId,
                                UserFullName = item.UserFullName,

                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                CategoryId = item.CategoryId,
                                CategoryName = item.CategoryName,
                                CasePlacingId = item.CasePlacingId
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userCases;
        }

        public async Task<List<CourtCaseVM>> GetLawyerCases(long? userId)
        {
            List<CourtCaseVM> userCases = new List<CourtCaseVM>();
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
                    var caseList = await (from u in ctx.UserProfiles
                                          join c in ctx.CourtCases on u.UserId equals c.LawyerId
                                          join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                          where u.UserId == userId && u.RoleId == (int)Roles.Lawyer && !(u.IsDeleted && c.IsDeleted) && c.StatusId == (int)ECaseStatuses.Approved
                                          select new CourtCaseVM
                                          {
                                              UserId = userId,
                                              UserFullName = u.FullName,
                                              CaseId = c.CaseId,
                                              CitizenId = c.CitizenId,
                                              LawyerId = c.LawyerId,
                                              CaseNumber = c.CaseNumber,
                                              CaseTitle = c.CaseTitle,
                                              CategoryId = c.CategoryId,
                                              CategoryName = cat.CategoryName
                                          })
                      .ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            userCases.Add(new CourtCaseVM
                            {
                                UserId = userId,
                                UserFullName = item.UserFullName,

                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                CategoryId = item.CategoryId,
                                CategoryName = item.CategoryName
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userCases;
        }

        public async Task<List<CaseDetailVM>> GetCitizenDateList(long? userId, long caseId)
        {

            List<CaseDetailVM> caseDetails = new List<CaseDetailVM>();
            try
            {
                //if (!isAuthenticated) return caseDetails;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId)
                //        return caseDetails;
                //}

                if (userId != null && userId > 0)
                {
                    var caseList =
                       await (from cc in ctx.CourtCases
                              join cd in ctx.CasesDetails on cc.CaseId equals cd.CaseId
                              join cdd in ctx.CasesDocuments on cd.ID equals cdd.CaseDetailId into cddJoined
                              from cdd in cddJoined.DefaultIfEmpty()
                              where cc.CitizenId == userId && cc.CaseId == caseId && !cc.IsDeleted && !cd.IsDeleted && (cdd == null || cdd.DocTypeId == (int)EDocumentType.CaseDateDocument)
                              select new
                              {
                                  cd.ID,
                                  cc.CaseId,
                                  cc.CitizenId,
                                  cc.LawyerId,
                                  cc.CaseNumber,
                                  cc.CaseTitle,
                                  cd.CaseDateTitle,
                                  cd.HearingDate,
                                  cd.DateDescription,
                                  cd.CaseStatusId,
                                  DocName = cdd != null ? cdd.DocName : null,
                                  cd.CreatedDate
                              })
                               .GroupBy(x => x.ID)
                    .Select(group => group.First())
                    .ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            caseDetails.Add(new CaseDetailVM
                            {
                                HearingDateId=item.ID,
                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                DateTitle = item.CaseDateTitle,
                                HearingDate = item.HearingDate,
                                DateDescription = item.DateDescription,
                                CaseStatusId = item.CaseStatusId,
                                DocName = item.DocName,
                               CreatedDate =item.CreatedDate,
                                DateFiles = await documentService.FilesCollections(new FileParmsVM() { DateId = item.ID, DocType = (int)EDocumentType.CaseDateDocument })
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetails;
        }
        public async Task<List<CaseDetailVM>> GetLawyerDateList(long? userId, long caseId)
        {

            List<CaseDetailVM> caseDetails = new List<CaseDetailVM>();
            try
            {
                //if (!isAuthenticated) return caseDetails;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId)
                //        return caseDetails;
                //}

                if (userId != null && userId > 0)
                {
                    var caseList =
                       await (from cc in ctx.CourtCases
                              join cd in ctx.CasesDetails on cc.CaseId equals cd.CaseId
                              join cdd in ctx.CasesDocuments on cd.ID equals cdd.CaseDetailId into cddJoined
                              from cdd in cddJoined.DefaultIfEmpty()
                              where cc.LawyerId == userId && cc.CaseId == caseId && !cc.IsDeleted && !cd.IsDeleted && (cdd == null || cdd.DocTypeId == (int)EDocumentType.CaseDateDocument)
                              select new
                              {
                                  cd.ID,
                                  cc.CaseId,
                                  cc.CitizenId,
                                  cc.LawyerId,
                                  cc.CaseNumber,
                                  cc.CaseTitle,
                                  cd.CaseDateTitle,
                                  cd.HearingDate,
                                  cd.DateDescription,
                                  cd.CaseStatusId,
                                  DocName = cdd != null ? cdd.DocName : null,
                                  cd.CreatedDate,
                              })
                              .GroupBy(x => x.ID)
                    .Select(group => group.First())
                    .ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            caseDetails.Add(new CaseDetailVM
                            {
                                HearingDateId=item.ID,
                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                DateTitle = item.CaseDateTitle,
                                HearingDate = item.HearingDate,
                                DateDescription = item.DateDescription,
                                CaseStatusId = item.CaseStatusId,
                                CreatedDate=item.CreatedDate,
                                //DocName = item.DocName
                                DateFiles = await documentService.FilesCollections(new FileParmsVM() { DateId= item.ID,DocType=(int)EDocumentType.CaseDateDocument })
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetails;
        }

        public async Task AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject)
        {
            try
            {
                //if (!isAuthenticated) return;

                if (acceptReject != null)
                {
                    var casse = await ctx.CourtCases.FindAsync(acceptReject.CaseId);
                    if (casse != null)
                    {
                        //get case statuses by case category 
                        var InitialStatus=await ctx.CategoriesStatuses.Where(c => c.CategoryId==casse.CategoryId && c.IsInitialStatus && !c.IsDeleted).FirstOrDefaultAsync();

                        casse.LegalStatusId = (acceptReject.Status == (int)ECaseStatuses.LawyerAccepted) ? InitialStatus.CaseStatusId : (int)ELeagalStatus.None;
                        casse.StatusId = acceptReject.Status;
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Occure while executing AcceptRejectCaseByLawyer " + ex.Message);
            }
        }

        public async Task<bool> AcceptRejectCase(AcceptRejectCaseVM acceptRejectVm)
        {
            try
            {
                //if (!isAuthenticated) return false; 

                if (acceptRejectVm is not null)
                {
                    var casse = await ctx.CourtCases.FindAsync(acceptRejectVm.CaseId);
                    if (casse != null)
                    {
                        var InitialStatus = await ctx.CategoriesStatuses.Where(c => c.CategoryId == casse.CategoryId && c.IsInitialStatus && !c.IsDeleted).FirstOrDefaultAsync();

                        //the below code will be checked in future
                        casse.StatusId = (acceptRejectVm.Status == (int)ECaseStatuses.LawyerAccepted)
                               ? (int)ECaseStatuses.Approved
                               : (acceptRejectVm.Status == (int)ECaseStatuses.AdminAccepted) ? (int)ECaseStatuses.ForwardedToLawyer : acceptRejectVm.Status;
                        casse.LegalStatusId = (acceptRejectVm.Status == (int)ECaseStatuses.LawyerAccepted)? InitialStatus.CaseStatusId: (int)ELeagalStatus.None;
                        casse.AcceptanceDate= acceptRejectVm.Status==(int)ECaseStatuses.Approved? DateTime.UtcNow:null;
                        await ctx.SaveChangesAsync();


                        //Create message pipline with Lawyer-Citizen
                        if (casse.StatusId==(int)ECaseStatuses.Approved)
                        {
                            var getAdmin = await ctx.UserProfiles.FirstOrDefaultAsync(x => x.RoleId == (int)Roles.Admin);
                            var getLawyer = await ctx.UserProfiles.FirstOrDefaultAsync(x =>x.UserId==casse.LawyerId && x.RoleId == (int)Roles.Lawyer);
                            var res =await messageRepository.Create(new MessageVm()
                            {
                                FromUserId = casse.LawyerId,
                                ToUserId = casse.CitizenId,
                                Content = string.Format(config["AppConfig:LawyerGreetingMsg"],getLawyer.FullName),
                                IsRead = false,
                                ParentId = -1
                            });
                            await chatHub.DirectMessage(res);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Occure while executing AcceptRejectCaseByAdmin " + ex.Message);
                return false;
            }
            return true;
        }

        public async Task<CaseDateResponseVM> CreateCaseDate(CaseDateVM detailVM)
        {
            CaseDateResponseVM caseDetailVM = new CaseDateResponseVM();
            try
            {

                if (detailVM != null)
                {
                    var checkCaseExist = await ctx.CourtCases.FindAsync(detailVM.CaseId);
                    if (checkCaseExist != null)
                    {
                        CasesDetail casesDetail = new CasesDetail()
                        {
                            CaseId = detailVM.CaseId,
                            CaseStatusId = detailVM.CaseStatusId,
                            CaseDateTitle = detailVM.DateTitle,
                            DateDescription = detailVM.DateDescription,
                            HearingDate = detailVM.HearingDate
                        };
                        if (checkCaseExist.LegalStatusId != detailVM.CaseStatusId)
                        {
                            checkCaseExist.LegalStatusId = detailVM.CaseStatusId;
                        }
                        await ctx.AddAsync(casesDetail);
                        await ctx.SaveChangesAsync();
                        caseDetailVM.HearingDateId = casesDetail.ID;

                        #region Case DateNotification
                        try
                        {
                        var getLawyer = await ctx.UserProfiles.FirstOrDefaultAsync(x => x.UserId == checkCaseExist.LawyerId && !x.IsDeleted);
                        var getCitizen = await ctx.UserProfiles.FirstOrDefaultAsync(x => x.UserId == checkCaseExist.CitizenId && !x.IsDeleted);
                        string body = $"Case. {checkCaseExist.CaseTitle}: The next hearing is scheduled for {casesDetail.HearingDate}.";
                        await _emailService.SendMailTrapEmail($"{detailVM?.DateTitle}", body, getCitizen?.Email);

                        //Send Notification
                        NotificationVm vm = new NotificationVm();
                        vm.NotificationType = (int)NotificationType.CaseDate;
                        vm.ToUserId = checkCaseExist.CitizenId;
                        vm.FromUserId = checkCaseExist.LawyerId;
                        vm.Content = body;
                        vm.ImageUrl = getCitizen.ProfilePicUrl;

                       await _notification.BroadCastAndSaveNotification(vm);
                        }
                        catch (Exception)
                        {
                        }
                        #endregion
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetailVM;
        }
        
        public async Task<CaseDateVM> UpdateCaseDate(CaseDateVM detailVM)
        {
            CaseDateVM caseDetailVM = new CaseDateVM();
            try
            {

                if (detailVM != null)
                {
                    var checkCaseExist = await ctx.CourtCases.Where(x=>x.CaseId==detailVM.CaseId && !x.IsDeleted).FirstOrDefaultAsync();
                    var GetDate = await ctx.CasesDetails.Where(x => x.ID == detailVM.CaseDateId && x.CaseId==checkCaseExist.CaseId && !x.IsDeleted).FirstOrDefaultAsync();
                    if (GetDate!=null)
                    {
                        GetDate.CaseStatusId = detailVM.CaseStatusId;
                        GetDate.CaseDateTitle = detailVM.DateTitle;
                        GetDate.DateDescription = detailVM.DateDescription;
                        GetDate.HearingDate = detailVM.HearingDate;

                        if (checkCaseExist.LegalStatusId != detailVM.CaseStatusId)
                        {
                            checkCaseExist.LegalStatusId = detailVM.CaseStatusId;
                        }
                         ctx.Entry(GetDate).State=EntityState.Modified;
                        await ctx.SaveChangesAsync();
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetailVM;
        }
        public async Task<CaseDateVM> GetCaseDate(long CaseId,long DateId)
        {
            CaseDateVM caseDetailVM = new CaseDateVM();
            try
            {
                    var GetDate = await ctx.CasesDetails.Where(x => x.ID == DateId && x.CaseId == CaseId && !x.IsDeleted).FirstOrDefaultAsync();

                if (GetDate!=null)
                {
                    caseDetailVM.CaseDateId = GetDate.ID;
                    caseDetailVM.DateTitle = GetDate.CaseDateTitle;
                    caseDetailVM.CaseId = GetDate.CaseId;
                    caseDetailVM.CaseStatusId = GetDate.CaseStatusId;
                    caseDetailVM.DateDescription= GetDate.DateDescription;
                    caseDetailVM.HearingDate= GetDate.HearingDate;

                    //case date documents
                    caseDetailVM.DateFiles = await documentService.FilesCollections(new FileParmsVM() {DocType=(int)EDocumentType.CaseDateDocument,DateId=GetDate.ID });

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetailVM;
        }
        public async Task<List<CourtCaseVM>> GetCasesForAdminApproval()
        {
            var adminCases = await (from c in ctx.CourtCases
                                    join u in ctx.UserProfiles on c.CitizenId equals u.UserId
                                    join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                    where (c.StatusId == (int)ECaseStatuses.New || c.StatusId == (int)ECaseStatuses.LawyerRejected) && !u.IsDeleted && !c.IsDeleted
                                    select new CourtCaseVM
                                    {
                                        UserFullName = u.FullName,
                                        CaseId = c.CaseId,
                                        CitizenId = c.CitizenId,
                                        LawyerId = c.LawyerId,
                                        CaseNumber = c.CaseNumber,
                                        CaseTitle = c.CaseTitle,
                                        CaseDescription = c.CaseDescription,
                                        CategoryId = c.CategoryId,
                                        CategoryName = cat.CategoryName,
                                        CaseStatusId = c.LegalStatusId,
                                        CasePlacingId = c.CasePlacingId,
                                        CreatedDate = c.CreatedDate
                                    })
                .ToListAsync();
            return adminCases;
        }
        public async Task<List<CourtCaseVM>> GetCasesForLawyerApproval(long lawywerId)
        {
            var adminCases = await (from c in ctx.CourtCases
                                    join u in ctx.UserProfiles on c.CitizenId equals u.UserId
                                    join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                    where (c.StatusId == (int)ECaseStatuses.AdminAccepted || c.StatusId == (int)ECaseStatuses.ForwardedToLawyer) 
                                    && c.LawyerId== lawywerId && !u.IsDeleted && !c.IsDeleted
                                    select new CourtCaseVM
                                    {
                                        UserFullName = u.FullName,
                                        CaseId = c.CaseId,
                                        CitizenId = c.CitizenId,
                                        LawyerId = c.LawyerId,
                                        CaseNumber = c.CaseNumber,
                                        CaseTitle = c.CaseTitle,
                                        CaseDescription = c.CaseDescription,
                                        CategoryId = c.CategoryId,
                                        CategoryName = cat.CategoryName,
                                        CaseStatusId = c.LegalStatusId,
                                        CasePlacingId = c.CasePlacingId
                                    })
                .ToListAsync();
            return adminCases;
        }

        public async  Task<bool> AssignEmployeeToCase(CourtCaseVM vm)
        {
           var _case = await ctx.CourtCases.FindAsync(vm.CaseId);

            if(_case is not null)
            {
                _case.AssignEmployeeId = vm.AssignEmployeeId;
               await ctx.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<CaseRejectionReason> AddCaseRejectionReason(AcceptRejectCaseVM vm) 
        {

            CaseRejectionReason crr = new CaseRejectionReason { 
              CaseId=vm.CaseId,
              Reason=vm.Reason,
              Status=vm.Status,
              RejectById=vm.DecisionUserId
            
            };
            ctx.CaseRejectionReasons.Add(crr);
             await ctx.SaveChangesAsync();

            return crr;
        }

        public async Task<List<CaseRejectionReasonVm>> GetCaseRejectionReason()
        {
           
           var _caseRejectionReason= await ctx.Database.SqlQuery<CaseRejectionReasonVm>($@"
                               select 
                                      cc.CaseId,
                                      cc.CaseNumber,
                                      cc.CaseTitle, 
                                      au.FirstName+' '+au.LastName as CitizenName,
                                      case_c.CategoryName,
                                      crr.Id as RejectionId,
                                      crr.Reason,
                                      crr.Status as CaseRejectionStatus 
                                 from CourtCases cc
                                 inner join AspNetUsers au
                                 on cc.CitizenId = au.Id
                                 inner join CaseCategories case_c
                                 on cc.CategoryId=case_c.ID
                                 inner join CaseRejectionReasons crr
                                 on cc.CaseId=crr.CaseId")
                                .ToListAsync();

            return _caseRejectionReason;
        }

        public async Task<CaseAprovalVM> GetAdminCaseDetails(long CaseId)
        {
            CaseAprovalVM CaseDetail = new CaseAprovalVM();
            try
            {

                if (CaseId != null)
                {
                    var SingleCase = await ctx.CourtCases.Where(c => c.CaseId == CaseId && c.StatusId == (int)ECaseStatuses.New && !c.IsDeleted).FirstOrDefaultAsync();

                    if (SingleCase != null)
                    {
                        CaseDetail.CaseDetails = new CourtCaseDetailVM()
                        {
                            CaseId = SingleCase.CaseId,
                            CaseTitle = SingleCase.CaseTitle,
                            CaseNumber = SingleCase.CaseNumber,
                            CaseStatusId = SingleCase.StatusId,
                            CaseStatus = Utils.GetEnumDescription((ECaseStatuses)SingleCase.StatusId),
                            CasePlacingId = SingleCase.CasePlacingId,
                            CasePlacing = Utils.GetEnumDescription((ECasePlacingType)SingleCase.CasePlacingId),
                            LegalStatusId = SingleCase.LegalStatusId,
                            LegalStatus = Utils.GetEnumDescription((ELeagalStatus)SingleCase.LegalStatusId),
                            CreatedDate = SingleCase.CreatedDate,
                            CaseFiles = await documentService.FilesCollections(new FileParmsVM() { CaseId = SingleCase.CaseId, DocType = (int)EDocumentType.CaseDocument })
                        };

                    var Citizen = await ctx.UserProfiles.Where(c => c.UserId == SingleCase.CitizenId && c.RoleId == (int)Roles.Citizen && c.IsActive == true && !c.IsDeleted).FirstOrDefaultAsync();

                    if (Citizen != null)
                    {
                        CaseDetail.CitizenDetail = new CaseCitizenDetailVM()
                        {
                            UserId = Citizen.UserId,
                            Email = Citizen.Email,
                            FullName = Citizen.FullName,
                            CNICNo = Citizen.CNICNo,
                            ContactNumber = Citizen.ContactNumber,
                            CurrAddress = Citizen.CurrAddress,
                            PermAddress = Citizen.PermAddress
                        };

                    }

                    var Lawyer = await ctx.UserProfiles.Where(c => c.UserId == SingleCase.LawyerId && c.RoleId == (int)Roles.Lawyer && c.IsActive == true && !c.IsDeleted).FirstOrDefaultAsync();

                    if (Lawyer != null)
                    {
                        CaseDetail.LawyerDetail = new CaseLawyerDetailVM()
                        {
                            UserId = Lawyer.UserId,
                            Email = Lawyer.Email,
                            FullName = Lawyer.FullName,
                            CNICNo = Lawyer.CNICNo,
                            ContactNumber = Lawyer.ContactNumber,
                            BarCouncilNo = Lawyer.BarCouncilNo,
                            CurrAddress = Lawyer.CurrAddress,
                            PermAddress = Lawyer.PermAddress
                        };

                    }

                    CaseDetail.PaymentDetail = "";
                }

                }






            }
            catch (Exception ex)
            {

                throw ex;
            }

            return CaseDetail;
        }

        public async Task<bool> UpdateCaseStatusByAdmin(AdminAcceptRejectCaseVM vM)
        {
            int result = 0;
            try
            {

                if (vM!=null)
                {
                    var SingleCase = await ctx
                        .CourtCases
                        .Where(c => c.CaseId == vM.CaseId && c.StatusId == (int)ECaseStatuses.New && !c.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (SingleCase != null)
                    {
                        SingleCase.StatusId= (int)(vM.CaseStatus?ECaseStatuses.AdminAccepted: ECaseStatuses.AdminRejected);
                        SingleCase.LawyerId=vM.LawyerId;
                        SingleCase.RedundantLawyerId=vM.RedundantLawyerId;
                        SingleCase.AssignEmployeeId = vM.EmployeeId;
                        SingleCase.RejectionId=vM.RejectionId;
                        SingleCase.RejectionNote = vM.RejectionNote;

                        ctx.Entry(SingleCase).State = EntityState.Modified;
                        result= await ctx.SaveChangesAsync();

                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return (result!=0);
        }
    
        /// <summary>
        /// Get City, Legal Status, Category and Location Wise Report
        /// </summary>
        /// <returns></returns>
        public async Task<List<CasesCityWiseReportVm>> GetCityCategoryLegalStatusReport(CasesCityWiseReportVm model) 
        {
           
            var qeryWithWhereClause = model.GenerateReportQueriesWithWhereClause();

            var reportList = await ctx.Database.SqlQueryRaw<CasesCityWiseReportVm>($"{qeryWithWhereClause}")
                  .ToListAsync();

            return reportList;
        }

        public async Task<List<CompletePendingCasesCountVm>> GetCompletedPendingCasesCount() 
        {
         var   caseCountByStatys = await ctx.Database.SqlQueryRaw<CompletePendingCasesCountVm>($@"
                                                     SELECT CASE
                                                     WHEN StatusId = 8 THEN
                                                         'Completed'
                                                     ELSE
                                                         'Pending'
                                                     END AS Status,
                                                     COUNT(1) AS Count
                                                     FROM dbo.CourtCases
                                                     WHERE IsDeleted = 0
                                                           AND StatusId != 6
                                                     GROUP BY CASE
                                                       WHEN StatusId = 8 THEN
                                                           'Completed'
                                                       ELSE
                                                           'Pending'
                                                   END                                          
                                                          		   ")
                                                                          .ToListAsync();
            return caseCountByStatys;
        }

        public async Task<List<FinancialTransactionReportVm>> GetFinancialTransactionReport(FinancialReportFilterVm filters)
        {
            
            var financialTransactionDetails = await ctx.Database.SqlQueryRaw<FinancialTransactionReportVm>($@"
                                                    
                                                               SELECT  
                                                                      lawyer.FullName AS Lawyer,
                                                                      citizen.FullName AS Citizen,
                                                                      cases.CaseTitle,
                                                                      cases.CreatedDate,
                                                                      SUM(transactions.Amount) as Amount,
                                                                      statues.Status
                                                               FROM dbo.UserProfiles citizen
                                                                   INNER JOIN dbo.CourtCases cases
                                                                       ON citizen.UserId = cases.CitizenId
                                                                   INNER JOIN dbo.UserProfiles lawyer
                                                                       ON cases.LawyerId = lawyer.UserId
                                                                   INNER JOIN Payments payment
                                                                       ON CitizenId = FromUserId
                                                                   INNER JOIN dbo.CaseStatuses statues
                                                                       ON cases.LegalStatusId = statues.StatusId
                                                                   INNER JOIN PaymentTransactions transactions
                                                                       ON payment.PaymentId = transactions.PaymentId
                                                               WHERE (
                                                                        @LawyerId IS NULL
                                                                         OR lawyer.UserId = @LawyerId
                                                                     )
                                                                     AND
                                                                     (
                                                                         citizen.UserId = @CitizenId
                                                                         OR @CitizenId IS NULL
                                                                     )
                                                                     AND
                                                                     (
                                                                         @CaseTitle IS NULL
                                                                         OR cases.CaseTitle LIKE '%' + @CaseTitle + '%'
                                                                     )
                                                                     AND
                                                                     (
                                                                         @LegalStatusId IS NULL
                                                                         OR cases.LegalStatusId = @LegalStatusId
                                                                     )
                                                                     AND
                                                                     (
                                                                         @FromDate IS NULL
                                                                         OR transactions.TransactionDate >= @FromDate
                                                                     )
                                                                     AND
                                                                     (
                                                                         @ToDate IS NULL
                                                                         OR transactions.TransactionDate <= @ToDate
                                                                     )
                                                               GROUP BY cases.CitizenId,
                                                                        lawyer.FullName,
                                                                        citizen.FullName,
                                                                        cases.CaseTitle,
                                                                        cases.CreatedDate,
                                                                        statues.Status,
                                                               		 cases.CaseId   ", 
                                                                     new SqlParameter("@LawyerId", filters.LawyerId?? SqlInt32.Null),
                                                                     new SqlParameter("@CitizenId",filters.CitizenId ?? SqlInt32.Null),
                                                                     new SqlParameter("@CaseTitle",filters.CaseTitle ?? SqlString.Null),
                                                                     new SqlParameter("@LegalStatusId",filters.LegalStatusId ?? SqlInt32.Null),
                                                                     new SqlParameter("@FromDate",filters.FromDate ?? SqlDateTime.Null),
                                                                     new SqlParameter("@ToDate",filters.ToDate ?? SqlDateTime.Null)
                                                                     
                                                                     )
                                                                         .ToListAsync();
            return financialTransactionDetails;
        }


        #region Admin

        public async Task<CaseDateResponseVM> CreateCaseDateByAdmin(NewCaseDateVM detailVM)
        {
            CaseDateResponseVM caseDetailVM = new CaseDateResponseVM();
            try
            {

                if (detailVM != null)
                {
                    var checkCaseExist = await ctx.CourtCases.FindAsync(detailVM.CaseId);
                    if (checkCaseExist != null)
                    {
                        CasesDetail casesDetail = new CasesDetail()
                        {
                            CaseId = detailVM.CaseId,
                            CaseStatusId = detailVM.CaseStatusId,
                            CaseDateTitle = detailVM.DateTitle,
                            DateDescription = detailVM.DateDescription,
                            HearingDate = detailVM.HearingDate
                        };
                        if (checkCaseExist.LegalStatusId != detailVM.CaseStatusId)
                        {
                            checkCaseExist.LegalStatusId = detailVM.CaseStatusId;
                        }
                        await ctx.AddAsync(casesDetail);
                        await ctx.SaveChangesAsync();
                        caseDetailVM.HearingDateId = casesDetail.ID;
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetailVM;
        }
        public async Task<NewCaseDateVM> UpdateCaseDateByAdmin(NewCaseDateVM detailVM)
        {
            NewCaseDateVM caseDetailVM = new NewCaseDateVM();
            try
            {

                if (detailVM != null)
                {
                    var checkCaseExist = await ctx.CourtCases.Where(x => x.CaseId == detailVM.CaseId && !x.IsDeleted).FirstOrDefaultAsync();
                    var GetDate = await ctx.CasesDetails.Where(x => x.ID == detailVM.CaseDateId && x.CaseId == checkCaseExist.CaseId && !x.IsDeleted).FirstOrDefaultAsync();
                    if (GetDate != null)
                    {
                        GetDate.CaseStatusId = detailVM.CaseStatusId;
                        GetDate.CaseDateTitle = detailVM.DateTitle;
                        GetDate.DateDescription = detailVM.DateDescription;
                        GetDate.HearingDate = detailVM.HearingDate;

                        if (checkCaseExist.LegalStatusId != detailVM.CaseStatusId)
                        {
                            checkCaseExist.LegalStatusId = detailVM.CaseStatusId;
                        }
                        ctx.Entry(GetDate).State = EntityState.Modified;
                        await ctx.SaveChangesAsync();
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetailVM;
        }
        public async Task<NewCaseDateVMWithFiles> GetCaseDateByAdmin(long CaseId, long DateId)
        {
            NewCaseDateVMWithFiles caseDetailVM = new NewCaseDateVMWithFiles();
            try
            {
                var GetDate = await ctx.CasesDetails.Where(x => x.ID == DateId && x.CaseId == CaseId && !x.IsDeleted).FirstOrDefaultAsync();

                if (GetDate != null)
                {
                    caseDetailVM.CaseDateId = GetDate.ID;
                    caseDetailVM.DateTitle = GetDate.CaseDateTitle;
                    caseDetailVM.CaseId = GetDate.CaseId;
                    caseDetailVM.CaseStatusId = GetDate.CaseStatusId;
                    caseDetailVM.DateDescription = GetDate.DateDescription;
                    caseDetailVM.HearingDate = GetDate.HearingDate;

                    //case date documents
                    caseDetailVM.DateFiles = await documentService.FilesCollections(new FileParmsVM() { DocType = (int)EDocumentType.CaseDateDocument, DateId = GetDate.ID });

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetailVM;
        }

        public async Task<List<GetCourtCaseListVM>> GetAllCasesByAdmin()
        {
           return await (from c in ctx.CourtCases
             join up in ctx.UserProfiles on c.CitizenId equals up.UserId
             join up1 in ctx.UserProfiles on c.LawyerId equals up1.UserId
             where c.StatusId == (int)(ECaseStatuses.AdminAccepted | ECaseStatuses.Approved) && !c.IsDeleted
             && !(up.IsDeleted && up1.IsDeleted)
             select new GetCourtCaseListVM
             {
                 CaseId=c.CaseId,
                 CaseDescription=c.CaseDescription,
                 CaseTitle=c.CaseTitle,
                 CitizenId=c.CitizenId,
                 CitizenName = up.FullName,
                 LawyerId=c.LawyerId,
                 LawyerName = up1.FullName,
                 CreatedDate=c.CreatedDate,
                 StatusId=c.StatusId,
                 ECaseStatus=Utils.GetEnumDescription((ECaseStatuses)c.StatusId),
                 LeagalStatusId=c.LegalStatusId,
                 LeagalStatus= Utils.GetEnumDescription((ELeagalStatus)c.LegalStatusId),
             }
             )
             .ToListAsync();
        }

        public async Task<List<CaseDetailVM>> GetDateListByCaseId(long? userId, long caseId)
        {

            List<CaseDetailVM> caseDetails = new List<CaseDetailVM>();
            try
            {
                //if (!isAuthenticated) return caseDetails;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId)
                //        return caseDetails;
                //}

                if (userId != null && userId > 0)
                {
                    var caseList =
                       await (from cc in ctx.CourtCases
                              join cd in ctx.CasesDetails on cc.CaseId equals cd.CaseId
                              join cdd in ctx.CasesDocuments on cd.ID equals cdd.CaseDetailId into cddJoined
                              from cdd in cddJoined.DefaultIfEmpty()
                              join cs in ctx.CaseStatuses on cd.CaseStatusId equals cs.StatusId
                              where cc.CaseId == caseId && !cc.IsDeleted && !cs.IsDeleted && (cdd == null || cdd.DocTypeId == (int)EDocumentType.CaseDateDocument)
                              select new
                              {
                                  cd.ID,
                                  cc.CaseId,
                                  cc.CitizenId,
                                  cc.LawyerId,
                                  cd.CaseDateTitle,
                                  cd.HearingDate,
                                  cd.DateDescription,
                                  cd.CaseStatusId,
                                  cs.Status
                              })
                               .GroupBy(x => x.ID)
                    .Select(group => group.First())
                    .ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            caseDetails.Add(new CaseDetailVM
                            {
                                HearingDateId = item.ID,
                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                DateTitle = item.CaseDateTitle,
                                HearingDate = item.HearingDate,
                                DateDescription = item.DateDescription,
                                CaseStatusId = item.CaseStatusId,
                                CaseDateStatus=item.Status
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetails;
        }

        public async Task<bool> DeleteCaseDateByAdmin(long userId, long caseId, long dateId)
        {
            return (userId > 0 && caseId > 0 && dateId > 0) &&
          (await ctx.CasesDetails
              .Where(x => x.ID == dateId && x.CaseId == caseId && !x.IsDeleted)
              .ExecuteUpdateAsync(c => c
                  .SetProperty(b => b.IsDeleted, true)
                  .SetProperty(b => b.UpdatedBy, userId)
                  .SetProperty(b => b.UpdateDate, DateTime.UtcNow)
              ) > 0);
        }

        public async Task<bool> DeleteCaseByAdmin(long userId, long caseId)
        {
            //if first query return more than 0 rows then second query will be executed
            return (userId > 0 && caseId > 0) &&
                (await ctx.CourtCases
              .Where(x => x.CaseId==caseId && !x.IsDeleted)
              .ExecuteUpdateAsync(c => c
                  .SetProperty(b => b.IsDeleted, true)
                  .SetProperty(b => b.UpdatedBy, userId)
                  .SetProperty(b => b.UpdateDate, DateTime.UtcNow)
              )>0)
              &&
            (await ctx.CasesDetails
              .Where(x => x.CaseId == caseId && !x.IsDeleted)
              .ExecuteUpdateAsync(c => c
                  .SetProperty(b => b.IsDeleted, true)
                  .SetProperty(b => b.UpdatedBy, userId)
                  .SetProperty(b => b.UpdateDate, DateTime.UtcNow)
              ) > 0);
        }

        public async Task<CaseAprovalVM> GetApprovedCaseDetailsByAdmin(long CaseId)
        {
            CaseAprovalVM CaseDetail = new CaseAprovalVM();
            try
            {

                if (CaseId != null)
                {
                    var SingleCase = await ctx.CourtCases.Where(c => c.CaseId == CaseId && c.StatusId != (int)ECaseStatuses.New && !c.IsDeleted).FirstOrDefaultAsync();

                    if (SingleCase != null)
                    {
                        CaseDetail.CaseDetails = new CourtCaseDetailVM()
                        {
                            CaseId = SingleCase.CaseId,
                            CaseTitle = SingleCase.CaseTitle,
                            CaseNumber = SingleCase.CaseNumber,
                            CaseStatusId = SingleCase.StatusId,
                            CaseStatus = Utils.GetEnumDescription((ECaseStatuses)SingleCase.StatusId),
                            CasePlacingId = SingleCase.CasePlacingId,
                            CasePlacing = Utils.GetEnumDescription((ECasePlacingType)SingleCase.CasePlacingId),
                            LegalStatusId = SingleCase.LegalStatusId,
                            LegalStatus = Utils.GetEnumDescription((ELeagalStatus)SingleCase.LegalStatusId),
                            CreatedDate = SingleCase.CreatedDate,
                            CaseFiles = await documentService.FilesCollections(new FileParmsVM() { CaseId = SingleCase.CaseId, DocType = (int)EDocumentType.CaseDocument })
                        };

                        var Citizen = await ctx.UserProfiles.Where(c => c.UserId == SingleCase.CitizenId && c.RoleId == (int)Roles.Citizen && c.IsActive == true && !c.IsDeleted).FirstOrDefaultAsync();

                        if (Citizen != null)
                        {
                            CaseDetail.CitizenDetail = new CaseCitizenDetailVM()
                            {
                                UserId = Citizen.UserId,
                                Email = Citizen.Email,
                                FullName = Citizen.FullName,
                                CNICNo = Citizen.CNICNo,
                                ContactNumber = Citizen.ContactNumber,
                                CurrAddress = Citizen.CurrAddress,
                                PermAddress = Citizen.PermAddress
                            };

                        }

                        var Lawyer = await ctx.UserProfiles.Where(c => c.UserId == SingleCase.LawyerId && c.RoleId == (int)Roles.Lawyer && c.IsActive == true && !c.IsDeleted).FirstOrDefaultAsync();

                        if (Lawyer != null)
                        {
                            CaseDetail.LawyerDetail = new CaseLawyerDetailVM()
                            {
                                UserId = Lawyer.UserId,
                                Email = Lawyer.Email,
                                FullName = Lawyer.FullName,
                                CNICNo = Lawyer.CNICNo,
                                ContactNumber = Lawyer.ContactNumber,
                                BarCouncilNo = Lawyer.BarCouncilNo,
                                CurrAddress = Lawyer.CurrAddress,
                                PermAddress = Lawyer.PermAddress
                            };

                        }

                        CaseDetail.PaymentDetail = "";
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return CaseDetail;
        }

        public async Task<CourtCases> CreateUpdateCaseByAdmin(AdminCourtCaseVM caseVM)
        {
            CourtCases courtCases = new CourtCases();
            try
            {
                //if (!isAuthenticated) return;

                if (caseVM != null)
                {
                    if (caseVM?.CaseId > 0)
                    {
                        var CheckCaseExist = await ctx.CourtCases.Where(x => x.CaseId == caseVM.CaseId && !x.IsDeleted).FirstOrDefaultAsync();
                        if (CheckCaseExist != null)
                        {
                            CheckCaseExist.CitizenId = caseVM.CitizenId;
                            CheckCaseExist.LawyerId = caseVM.LawyerId;
                            CheckCaseExist.RedundantLawyerId = caseVM.RedundantLawyerId;
                            CheckCaseExist.CaseNumber = caseVM.CaseNumber ?? "";
                            CheckCaseExist.CaseTitle = caseVM.CaseTitle;
                            CheckCaseExist.PartyId = caseVM.PartyId;
                            CheckCaseExist.CategoryId = caseVM.CategoryId;
                            CheckCaseExist.CaseDescription = caseVM.CaseDescription!;
                            CheckCaseExist.CaseJurisdictionId = caseVM.CaseJurisdictionId;
                            CheckCaseExist.CourtId = caseVM.CourtId;
                            CheckCaseExist.CasePlacingId = caseVM.CasePlacingId;
                            CheckCaseExist.LegalStatusId = (int)ELeagalStatus.None;
                            CheckCaseExist.StatusId = (int)ECaseStatuses.New;
                            CheckCaseExist.CasePlacedFor = caseVM.CasePlacedFor;
                            CheckCaseExist.AssignEmployeeId = caseVM.AssignEmployeeId;
                            //CheckCaseExist.StatusId = (int)ECaseStatuses.AdminAccepted;
                            //CheckCaseExist.LegalStatusId = (int)ELeagalStatus.None;

                            ctx.Entry(CheckCaseExist).State = EntityState.Modified;
                            await ctx.SaveChangesAsync();

                            courtCases = CheckCaseExist;
                        }
                    }
                    else
                    {
                        courtCases.CitizenId = caseVM.CitizenId;
                        courtCases.LawyerId = caseVM.LawyerId;
                        courtCases.RedundantLawyerId = caseVM.RedundantLawyerId;
                        courtCases.CaseNumber = caseVM.CaseNumber ?? "";
                        courtCases.CaseTitle = caseVM.CaseTitle;
                        courtCases.PartyId = caseVM.PartyId;
                        courtCases.CategoryId = caseVM.CategoryId;
                        courtCases.CaseDescription = caseVM.CaseDescription!;
                        courtCases.CaseJurisdictionId = caseVM.CaseJurisdictionId;
                        courtCases.CourtId = caseVM.CourtId;
                        courtCases.CasePlacingId = caseVM.CasePlacingId;
                        courtCases.LegalStatusId = (int)ELeagalStatus.None;
                        courtCases.StatusId = (int)ECaseStatuses.New;
                        courtCases.CasePlacedFor = caseVM.CasePlacedFor;
                        courtCases.AssignEmployeeId=caseVM.AssignEmployeeId;
                        courtCases.StatusId= (int)ECaseStatuses.AdminAccepted;
                        courtCases.LegalStatusId =(int)ELeagalStatus.None;

                        await ctx.AddAsync(courtCases);
                        await ctx.SaveChangesAsync();
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return courtCases;
        }

        #endregion

        public async Task<List<FetchCaseDateNotificationDataVm>> GetCaseNextDateReminderDataForEmail()
        {
            var courtDateDetails =await ctx.Database.SqlQueryRaw<FetchCaseDateNotificationDataVm>($@"
                                      SELECT citizen.FullName AS CitizeName,
                                               citizen.Email AS CitizenEmail,
                                               citizen.ContactNumber AS CitizenContactNumber,
                                               lawyer.FullName AS LawerName,
                                               lawyer.Email AS LawyerEmail,
                                               lawyer.ContactNumber AS LawyerContactNumber,
                                               CourtCasesDetails.CaseTitle,
                                               CourtCasesDetails.CaseDateTitle,
                                               CourtCasesDetails.CaseDescription,
                                               CourtCasesDetails.HearingDate,
                                               citizen.UserId AS CitizenId,
                                               CourtCasesDetails.LawyerId AS LawyerId,
                                               CourtCasesDetails.DateId AS CaseDateId,
                                               CourtCasesDetails.CaseId
                                        FROM
                                        (
                                            SELECT CourtCases.CaseId,
                                                   ID AS DateId,
                                                   CaseStatusId,
                                                   CitizenId,
                                                   LawyerId,
                                                   CaseDateTitle,
                                                   CaseTitle,
                                                   CaseNumber,
                                                   CaseDescription,
                                                   HearingDate
                                            FROM dbo.CasesDetails
                                                INNER JOIN dbo.CourtCases
                                                    ON CasesDetails.CaseId = CourtCases.CaseId
                                            WHERE HearingDate > GETDATE()
                                                  AND FORMAT(HearingDate, 'yyyy/MM/dd') <= FORMAT(DATEADD(DAY, 2, GETDATE()), 'yyyy/MM/dd')
                                                  AND ID NOT IN
                                                      (
                                                          SELECT CaseDateId FROM EmailLogs WHERE issent = 1 AND IsDeleted = 0
                                                      )
                                        ) AS CourtCasesDetails
                                            INNER JOIN dbo.UserProfiles citizen
                                                ON citizen.UserId = CourtCasesDetails.CitizenId
                                            INNER JOIN dbo.UserProfiles lawyer
                                                ON lawyer.UserId = CourtCasesDetails.LawyerId").ToListAsync();
            return courtDateDetails;
        } 
        public async Task<List<FetchCaseDateNotificationDataVm>> GetCaseNextDateReminderDataForSms() 
        {
            var courtDateDetails = await ctx.Database.SqlQueryRaw<FetchCaseDateNotificationDataVm>($@"
                                      SELECT  
         citizen.FullName AS CitizeName,
         citizen.Email AS CitizenEmail,
         citizen.ContactNumber AS CitizenContactNumber,
         lawyer.FullName AS LawerName,
         lawyer.Email AS LawyerEmail,
         lawyer.ContactNumber AS LawyerContactNumber,
         CourtCasesDetails.CaseTitle,
         CourtCasesDetails.CaseDateTitle,
         CourtCasesDetails.CaseDescription,
         CourtCasesDetails.HearingDate,
         citizen.UserId AS CitizenId,
         CourtCasesDetails.LawyerId AS LawyerId,
         CourtCasesDetails.DateId AS CaseDateId,
         CourtCasesDetails.CaseId
  FROM
  (
      SELECT CourtCases.CaseId,
             ID AS DateId,
             CaseStatusId,
             CitizenId,
             LawyerId,
             CaseDateTitle,
             CaseTitle,
             CaseNumber,
             CaseDescription,
             HearingDate
      FROM dbo.CasesDetails
          INNER JOIN dbo.CourtCases
              ON CasesDetails.CaseId = CourtCases.CaseId
      WHERE HearingDate > GETDATE()
            AND FORMAT(HearingDate, 'yyyy/MM/dd') <= FORMAT(DATEADD(DAY, 2, GETDATE()), 'yyyy/MM/dd')
            AND ID NOT IN
                (
                    SELECT CaseDateId FROM smslogs WHERE issent = 1 AND IsDeleted = 0
                )
  ) AS CourtCasesDetails
      INNER JOIN dbo.UserProfiles citizen
          ON citizen.UserId = CourtCasesDetails.CitizenId
      INNER JOIN dbo.UserProfiles lawyer
          ON lawyer.UserId = CourtCasesDetails.LawyerId").ToListAsync();
            return courtDateDetails;
        }
    }

    


}

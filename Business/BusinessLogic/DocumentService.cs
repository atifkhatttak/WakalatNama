using Azure.Core;
using Business.Enums;
using Business.Helpers;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class DocumentService : IDocumentServiceV2
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        private readonly string LoggedInRole = "";
        private readonly ILogger<DocumentService> logger;
        private readonly WKNNAMADBCtx _ctx;
        public DocumentService(IHttpContextAccessor httpContextAccessor, ILogger<DocumentService> _logger, WKNNAMADBCtx ctx)
        {
            this._httpContextAccessor = httpContextAccessor;
            this.isAuthenticated = _httpContextAccessor?.HttpContext?.User.Identity.IsAuthenticated ?? false;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
            logger = _logger;
            _ctx = ctx;
        }
        public async Task<byte[]> Download(string filePath)
        {
            return await System.IO.File.ReadAllBytesAsync(filePath);
        }
        public async Task<string> UploadDocument(IFormFile file, string eDocumentType)
        {
            string fileUrl = "";
            string appRoot = "";
            try
            {
                appRoot = Directory.GetCurrentDirectory();
                appRoot = $@"{appRoot}\Uploads\{eDocumentType}";
                //create folder if not exist
                if (!Directory.Exists(appRoot))
                    Directory.CreateDirectory(appRoot);

                //get file extension
                FileInfo fileInfo = new FileInfo(file.FileName);
                string fileName = $"{LoggedInUserId}_{Path.GetFileNameWithoutExtension(fileInfo.Name)}_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";
                string FileFullName = fileName + fileInfo.Extension;

                string fileNameWithPath = Path.Combine(appRoot, FileFullName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                fileUrl = $@"/{eDocumentType}/{FileFullName}";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
            return fileUrl;
        }

        public async Task<List<UploadDocmentResponse>> UploadMultipleDocuments(UploadDocumentVmWrapper model)
        {
            List<UploadDocmentResponse> UploadedFiles = new List<UploadDocmentResponse>();

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 8
            };

            await Parallel.ForEachAsync(model.Files, options, async (fileVm,ct)=>
            {
               var response = await UploadDocument(fileVm.File, fileVm.DocumentType);
                UploadedFiles.Add(new UploadDocmentResponse { URL=response, DocumentType= fileVm.DocumentType, FileName= Path.GetFileNameWithoutExtension(fileVm.File.FileName), Extension = Path.GetExtension(fileVm.File.FileName), Length= fileVm.File.Length });
            });

            return UploadedFiles;
        }
        public List<string> GetAllFiles(string folderName)
        {
            var appRoot = Directory.GetCurrentDirectory();
            appRoot = $@"{appRoot}\Uploads\{folderName}";
            DirectoryInfo d = new DirectoryInfo(appRoot);

            return d.GetFiles()?.Select(x => x.FullName).ToList();
        }
        public Task<string> DeleteFile(string filePath)
        {
            File.Delete(filePath);
            return Task.FromResult(string.Empty);
        }

        public async Task<List<FilesCollectionVM>> FilesCollections(FileParmsVM file)
        {
            List<FilesCollectionVM> files = new List<FilesCollectionVM>();
            var d = (EDocumentType)file.DocType;
            try
            {
                switch (d)
                {
                    case EDocumentType.CaseDateDocument:
                        files = await _ctx
                   .CasesDocuments
             .Where(x => x.CaseDetailId == file.DateId && x.DocTypeId == (int)EDocumentType.CaseDateDocument && !x.IsDeleted)
             .Select(s => new FilesCollectionVM()
             {
                 DocId = s.DocumentId,
                 DocName = s.DocName,
                 DocUrl = s.DocPath,
                 DocSize = s.DocSize ?? 0,
                 DocType = s.DocTypeId
             }).ToListAsync();
                        break;

                    case EDocumentType.CaseDocument:
                        files = await _ctx
                   .CasesDocuments
             .Where(x => x.CaseId == file.CaseId && x.DocTypeId == (int)EDocumentType.CaseDocument && !x.IsDeleted)
             .Select(s => new FilesCollectionVM()
             {
                 DocId = s.DocumentId,
                 DocName = s.DocName,
                 DocUrl = s.DocPath,
                 DocSize = s.DocSize ?? 0,
                 DocType = s.DocTypeId
             }).ToListAsync();
                        break;

                    default:
                        break;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return files;
        }

        public async Task SaveDocument(ResponseUploadDocumentVmWrapper model)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 8
            };
            #region  Profile  Documents
            if (model.RequestType == (int)RequestType.Profile)
           {
                var userProfile =await  _ctx.UserProfiles.FirstOrDefaultAsync(x => x.UserId == model.Id && !x.IsDeleted);

                if(userProfile is not null)
                {
                    await Parallel.ForEachAsync(model.Files, options, async (fileVm, ct) =>
                    {
                        if (fileVm.DocumentType == EDocumentType.ProfilePic.GetDescription())
                            userProfile.ProfilePicUrl = fileVm.URL;
                        else if (fileVm.DocumentType == EDocumentType.NicFront.GetDescription())
                            userProfile.CNICFrontUrl = fileVm.URL;
                        else if (fileVm.DocumentType == EDocumentType.NicBack.GetDescription())
                            userProfile.CNICBackUrl = fileVm.URL;
                        else if (fileVm.DocumentType == EDocumentType.NICOPFront.GetDescription())
                            userProfile.NICOP = fileVm.URL;
                        else if (fileVm.DocumentType == EDocumentType.BarCouncilCardScanBack.GetDescription())
                            userProfile.BarCouncilBackUrl = fileVm.URL;
                        else if (fileVm.DocumentType == EDocumentType.BarCouncilCardScanFront.GetDescription())
                            userProfile.BarCouncilFrontUrl = fileVm.URL;

                    });
                }
                await _ctx.SaveChangesAsync();
            }
            #endregion

            #region Courst Cases Document
            else if (model.RequestType == (int)RequestType.CourtCase) 
            {
                //Lets soft delete existing files if exist
                await DeleteExistingFile(model);
                

                List<CasesDocument> courtCasesDocs = new List<CasesDocument>();

                await Parallel.ForEachAsync(model.Files,options,async (fileVm,ct) =>
                    {
                        courtCasesDocs.Add(new CasesDocument
                        {
                            CaseId = model.Id,
                            DocName = fileVm.FileName,
                            DocExtension = fileVm.Extension,
                            DocPath = fileVm.URL,
                            DocTypeId = (int)((EDocumentType)Enum.Parse(typeof(EDocumentType), fileVm.DocumentType)),
                            IsUploaded = true,
                            DocSize= fileVm.Length,
                            CaseDetailId= model.CaseDetaisId
                        });
                    });
             await   _ctx.CasesDocuments.AddRangeAsync(courtCasesDocs);
               await _ctx.SaveChangesAsync();
               
            }
            #endregion

            #region User
            else if (model.RequestType == (int)RequestType.User)
            {
                List<UserDocument> courtCasesDocs = new List<UserDocument>(); 

                await Parallel.ForEachAsync(model.Files, options, async (fileVm, ct) =>
                {
                    courtCasesDocs.Add(new UserDocument
                    {
                        UserId = model.Id,
                        DocName = fileVm.FileName,
                        DocExtension = fileVm.Extension,
                        DocPath = fileVm.URL,
                        DocTypeId = (int)((EDocumentType)Enum.Parse(typeof(EDocumentType), fileVm.DocumentType)),
                        IsUploaded = true,
                      //  DocSize = fileVm.Length
                    });
                });
                await _ctx.UserDocuments.AddRangeAsync(courtCasesDocs);
                await _ctx.SaveChangesAsync();

            }
            #endregion
        }

        #region private functions


        private async Task<bool> DeleteExistingFile(ResponseUploadDocumentVmWrapper model)
        {
            int count = 0;
            try
            {
                if (model?.RequestType == (int)RequestType.CourtCase)
                {
                        bool isCaseDate = (model.Id > 0 && model.CaseDetaisId > 0);
                        var docs = isCaseDate ?
                            await _ctx.CasesDocuments.Where(x => x.CaseId == model.Id && x.CaseDetailId == model.CaseDetaisId && !x.IsDeleted).ToListAsync()
                            : await _ctx.CasesDocuments.Where(x => x.CaseId == model.Id && !x.IsDeleted).ToListAsync();

                        if (docs.Any())
                        {
                            foreach (var item in docs)
                            {
                                var sql = "update casesdocuments set IsDeleted = 1, UpdateDate = @UpdateDate, UpdatedBy = @UpdatedBy where IsDeleted = 0 and DocumentId = @DocumentId";
                                count += await _ctx.Database.ExecuteSqlRawAsync(sql,
                                     new SqlParameter("@UpdateDate", DateTime.Now),
                                     new SqlParameter("@UpdatedBy", LoggedInUserId),
                                     new SqlParameter("@DocumentId", item.DocumentId));
                            }
                        }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return count>0;
        }
        #endregion
    }
}

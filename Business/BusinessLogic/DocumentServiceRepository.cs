using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class DocumentServiceRepository : IDocumentService
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        private readonly string LoggedInRole = "";
        DocumentServiceRepository(WKNNAMADBCtx ctx, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {

            this.ctx = ctx;
            this.config = config;
            _httpContextAccessor = httpContextAccessor;
            this.isAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirstValue("role")) : -1;
        }


        public Task<Google.Apis.Drive.v3.Data.File> CreateFolder(string folderName)
        {
            throw new NotImplementedException();
        }

        public Task Delete(object id)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string fileId)
        {
            throw new NotImplementedException();
        }

        public MemoryStream DownloadFile(string fileId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDocument>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<UserDocument> GetById(object id)
        {
            throw new NotImplementedException();
        }

      

        public Task<IEnumerable<Google.Apis.Drive.v3.Data.File>> GetFiles(string folder)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Insert(UserDocument obj)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(UserDocument obj)
        {
            throw new NotImplementedException();
        }

        public Task<Google.Apis.Drive.v3.Data.File> UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription)
        {
            throw new NotImplementedException();
        }
    }
}

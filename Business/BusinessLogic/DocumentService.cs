using Azure.Core;
using Business.Enums;
using Business.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class DocumentService : IDocumentServiceV2
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DocumentService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<byte[]> Download(string filePath)
        {
            return  await System.IO.File.ReadAllBytesAsync(filePath);
        }
        public async Task <string> UploadDocument(IFormFile file,string eDocumentType)
        {
            //var request = _httpContextAccessor.HttpContext.Request;
            //string baseUrl = $"{request.Scheme}://{request.Host}";
            var appRoot = Directory.GetCurrentDirectory();
            appRoot = $@"{appRoot}\Uploads\{eDocumentType}";

            //create folder if not exist
            if (!Directory.Exists(appRoot))
                Directory.CreateDirectory(appRoot);

            //get file extension
            FileInfo fileInfo = new FileInfo(file.FileName);
            string fileName =Path.GetFileNameWithoutExtension(fileInfo.Name)+$"_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}" + fileInfo.Extension;

            string fileNameWithPath = Path.Combine(appRoot, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
              await  file.CopyToAsync(stream);
            }

            string fileUrl = "https://wakalatnaama.somee.com/Uploads/" + eDocumentType + "/" + fileName;

            //return fileUrl; 
            return fileNameWithPath;
        }
        public List<string> GetAllFiles(string folderName)
        {
            var appRoot = Directory.GetCurrentDirectory();
            appRoot = $@"{appRoot}\Uploads\{folderName}";
            DirectoryInfo d = new DirectoryInfo(appRoot);

            return    d.GetFiles()?.Select(x => x.FullName).ToList();
        }
        public Task<string> DeleteFile(string filePath)
        {
            File.Delete(filePath);
            return Task.FromResult(string.Empty);
        }
    }
}

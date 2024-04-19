using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IDocumentServiceV2
    {
        public Task<string> UploadDocument(IFormFile file, string eDocumentType);  
        public Task<Byte[]> Download(string filePath);
        public Task<string> DeleteFile(string filePath); 
        public List<string> GetAllFiles(string path);
    }
}

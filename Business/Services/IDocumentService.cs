using Data.DomainModels;
using Google.Apis.Drive.v3;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IDocumentService : IBaseRepository<UserDocument>
    {
        public  void Init();
        public Task<Google.Apis.Drive.v3.Data.File> CreateFolder(string folderName);
        public Task<Google.Apis.Drive.v3.Data.File> UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription);
        public void DeleteFile(string fileId);
        public Task<IEnumerable<Google.Apis.Drive.v3.Data.File>> GetFiles(string folder);

        public MemoryStream DownloadFile(string fileId);  
    }
}

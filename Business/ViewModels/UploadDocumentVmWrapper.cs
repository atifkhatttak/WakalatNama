using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class UploadDocumentVmWrapper
    {
        public long Id { get; set; }
        public int RequestType { get; set; }
        public long? CaseDetailsId { get; set; }

        public List<UploadDocumentVm> Files { get; set; } 

    }

    public class ResponseUploadDocumentVmWrapper 
    {
        public long Id { get; set; }
        public int RequestType { get; set; }
        public long? CaseDetaisId { get; set; }

        public List<UploadDocmentResponse> Files { get; set; } 

    }

    public class UploadDocumentVm
    {
        public string DocumentType { get; set; }
        public IFormFile File { get; set; }
    }

    
    public class UploadDocmentResponse
    {
        public   string URL { get; set; }
        public   string DocumentType { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public long Length { get; set; }
        public long? CaseDetailsId { get; set; } 
    }
}

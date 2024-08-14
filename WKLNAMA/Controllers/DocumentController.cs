using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.ObjectModel;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : BaseController<UserDocument>
    {
        private readonly IDocumentService documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly IDocumentServiceV2 _documentUploadService;
        private readonly IUserRepository _userRepository;

        public DocumentController(IDocumentService documentService, IHttpContextAccessor httpContextAccessor, ILogger<DocumentController> logger, IDocumentServiceV2 documentUploadService ) : base(documentService, httpContextAccessor)
        {
            this.documentService = documentService;
            _logger = logger;
            _documentUploadService = documentUploadService;
        }

        [HttpPost("UploadFile")]
        public async Task<ActionResult> UploadFile(IFormFile file, string docType)
        {
            var path = await _documentUploadService.UploadDocument(file, docType);
            var filePath = $"{this.Request.Scheme}://{this.Request.Host}/resources{path}";

            return Ok(Task.FromResult(filePath));
        }

        [HttpPost("UploadMultipleFile")]
        public async Task<ActionResult> Upload([FromForm]UploadDocumentVmWrapper files)
        {
            var response = await _documentUploadService.UploadMultipleDocuments(files);

            response = response.Select(x => new UploadDocmentResponse
            {
                URL = $"{this.Request.Scheme}://{this.Request.Host}/resources{x.URL}",
                DocumentType = x.DocumentType,
                FileName= x.FileName,
                Extension= x.Extension,
                Length= x.Length
    }
           ).ToList();

            var documentToSave = new ResponseUploadDocumentVmWrapper 
                      {
                          Id=files.Id,
                          RequestType=files.RequestType,
                          Files= response,
                          CaseDetaisId= files.CaseDetailsId
            };

            await  _documentUploadService.SaveDocument(documentToSave);

            return Ok(Task.FromResult(documentToSave));
        }

        [HttpGet("GetFiles")]
        public async Task<ActionResult> GetFiles(string folderName)
        {
            try
            {
                var filesPaths = _documentUploadService.GetAllFiles(folderName);

                var urls = filesPaths?.Select(x => x = $"{this.Request.Scheme}://{this.Request.Host}/resources{(x.Split("Uploads")[1]).Replace(@"\", "/")}");

                return Ok(Task.FromResult(urls));
            }
            catch (Exception ex)
            {
                return Ok(Task.FromResult($"Iinternal Server{Environment.NewLine}Message {ex.Message}{Environment.NewLine}{ex.StackTrace}"));
            }
        }

        [NonAction]
        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile(string fileId, string fileType, string fileName)
        {

            var file = documentService.DownloadFile(fileId);
            var ddd = new FileContentResult(file.ToArray(), fileType);
            ddd.FileDownloadName = fileName;
            return ddd;
        }
        [AllowAnonymous]
        [HttpGet("Download")]
        public IActionResult DownloadFile(string filePath)
        {
            var baseDirectory = $@"{Directory.GetCurrentDirectory()}\Uploads";

            if (!string.IsNullOrEmpty(filePath))
            {
                string[] contents = filePath.Split("resources");

                if (contents.Length > 1)
                {
                    contents[1] = contents[1].Replace(@"/", @"\");
                    baseDirectory += contents[1];
                }

                var file = _documentUploadService.Download(filePath);
            }


            return PhysicalFile(baseDirectory, MimeTypes.GetMimeType(baseDirectory), Path.GetFileName(baseDirectory));
        }

        [NonAction]
        [HttpPost("CreateFolder")]
        public async Task<ActionResult> CreateFolder(string folderName)
        {
            var folder = documentService.CreateFolder(folderName);
            return Ok(Task.FromResult(folder));
        }

        [AllowAnonymous]
        [HttpPost("DeleteFile")]
        public async Task<ActionResult> DeleteFile(string filePath)
        {
            var baseDirectory = $@"{Directory.GetCurrentDirectory()}\Uploads";

            if (!string.IsNullOrEmpty(filePath))
            {
                string[] contents = filePath.Split("resources");

                if (contents.Length > 1)
                {
                    contents[1] = contents[1].Replace(@"/", @"\");
                    baseDirectory += contents[1];
                }
            }
            await _documentUploadService.DeleteFile(baseDirectory);
            //documentService.DeleteFile(folderName);
            return Ok(Task.FromResult("File deleted Successfully"));
        }

        [NonAction]
        public override async Task<ActionResult> GetAll()
        {
            return Ok("");
        }
        [NonAction]
        public override async Task<ActionResult> Post(UserDocument T)
        {
            return Ok("");
        }
        [NonAction]
        public async override Task<ActionResult> GetById(Object _viewModel) { return Ok(); }

        [NonAction]
        public async override Task<ActionResult> Delete(object id) { return Ok(""); }
        [NonAction]
        public async override Task<ActionResult> Update(UserDocument _object) { return Ok(); }

    }
}

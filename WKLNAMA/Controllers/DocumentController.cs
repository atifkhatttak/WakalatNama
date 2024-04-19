using Business.Enums;
using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : BaseController<UserDocument>
    {
        private readonly IDocumentService documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly IDocumentServiceV2 _documentUploadService;

        public DocumentController(IDocumentService documentService, IHttpContextAccessor httpContextAccessor, ILogger<DocumentController> logger, IDocumentServiceV2 documentUploadService) : base(documentService, httpContextAccessor)
        {
            this.documentService = documentService;
            _logger = logger;
            _documentUploadService = documentUploadService;
        }

        [HttpPost("UploadFile")]
        public async Task<ActionResult> UploadFile(IFormFile file, string docType) 
        {
           var path=  await  _documentUploadService.UploadDocument(file, docType);
           //  var file = await documentService.UploadFile(_file.OpenReadStream(), _file.FileName, _file.ContentType, folderName, "This is testing file");
            return Ok(Task.FromResult(path));
        }

        [HttpGet("GetFiles")]
        public async Task<ActionResult> GetFiles(string folderName)
        {
            try
            {
              var filesPaths=   _documentUploadService.GetAllFiles(folderName);
                _logger.LogInformation($"Get  Files Method Started{Environment.NewLine}");
              //  var files =await documentService.GetFiles(_folderId);
                _logger.LogInformation($"Get File > API Service Call done  {Environment.NewLine}");
                return Ok(Task.FromResult(filesPaths));
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
            var file = _documentUploadService.Download(filePath);
            return PhysicalFile(filePath, MimeTypes.GetMimeType(filePath), Path.GetFileName(filePath));
        }

        [NonAction]
        [HttpPost("CreateFolder")]
        public async Task<ActionResult> CreateFolder(string folderName)
        {
          var folder=  documentService.CreateFolder(folderName);
            return Ok(Task.FromResult(folder));
        }

        [AllowAnonymous]
        [HttpPost("DeleteFile")]
        public async Task<ActionResult> DeleteFile(string filePath)
        {
           await _documentUploadService.DeleteFile(filePath);
            //documentService.DeleteFile(folderName);
            return Ok(Task.FromResult("File deleted Successfully"));
        }

        [NonAction]
        public override async Task<ActionResult> GetAll() {
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
        public async override Task<ActionResult> Delete(object id){ return Ok(""); }
        [NonAction]
        public async override Task<ActionResult> Update(UserDocument _object) { return Ok(); }

    }
}

using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : BaseController<UserDocument>
    {
        private readonly IDocumentService documentService;

        public DocumentController(IDocumentService documentService, IHttpContextAccessor httpContextAccessor) : base(documentService, httpContextAccessor)
        {
            this.documentService = documentService;
        }

        [AllowAnonymous]
        [HttpPost("UploadFile")]
        public async Task<ActionResult> UploadFile(IFormFile _file, string folderName)
        {

            var file = documentService.UploadFile(_file.OpenReadStream(), _file.FileName, _file.ContentType, folderName, "This is testing file");
            return Ok(Task.FromResult(file));
        }

        [AllowAnonymous]
        [HttpGet("GetFiles")]
        public async Task<ActionResult> GetFiles(string _folderId)
        {

            var files = documentService.GetFiles(_folderId);
            return Ok(Task.FromResult(files));
        }

        [AllowAnonymous]
        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile(string fileId, string fileType, string fileName)
        {

            var file = documentService.DownloadFile(fileId);
            var ddd = new FileContentResult(file.ToArray(), fileType);
            ddd.FileDownloadName = fileName;
            return ddd;
        }

        [AllowAnonymous]
        [HttpPost("CreateFolder")]
        public async Task<ActionResult> CreateFolder(string folderName)
        {
          var folder=  documentService.CreateFolder(folderName);
            return Ok(Task.FromResult(folder));
        }

        [AllowAnonymous]
        [HttpPost("DeleteFile")]
        public async Task<ActionResult> DeleteFile(string folderName = "14k7P51oCUAqaPMiqAzeiSi0ZsnUVWmuR")
        {
            documentService.DeleteFile(folderName);
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

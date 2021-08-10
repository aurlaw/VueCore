using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models;
using VueCore.Models.Domain;
using VueCore.Services;

namespace VueCore.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IMediator _mediatr;
        private readonly IDocumentService _docService;
        public DocumentController(ILogger<DocumentController> logger, IMediator mediatr, IDocumentService docService)
        {
            _logger = logger;
            _mediatr = mediatr;
            _docService = docService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("document/save")]
        public async Task<IActionResult> SaveDocument([FromBody]DocumentViewModel model, CancellationToken token) 
        {
            if(model == null)
            {
                return Ok(new {Success = false});
            }
            // Save the document.
            var document = await _docService.SaveDocumentAsync(model.Name, model.Notes, token);
            // Publish event
            await _mediatr.Publish(new NewDocumentReceived(document, null), token);

            return Ok(new {Success = true});
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 4000000)]
        [RequestSizeLimit(4000000)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken token) 
        {
            string title = string.Empty;
            string notes = string.Empty;
            if(Request.Headers.ContainsKey("x-vuecore-title"))
            {
                title = Request.Headers["x-vuecore-title"].ToString();
            } 
            else 
            {
                return Unauthorized("x-vuecore-title not set");
            }
            if(Request.Headers.ContainsKey("x-vuecore-notes"))
            {
                notes = Request.Headers["x-vuecore-notes"].ToString();
            } 
            else 
            {
                return Unauthorized("x-vuecore-notes not set");
            }
            if (file.Length > 0)
            {
                // Save the document.
                var document = await _docService.SaveDocumentAsync(title, notes, token);
                FileModel fileModel = null;
                using(var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms, token);
                    ms.Position = 0;

                    fileModel = new FileModel(file.FileName, file.ContentType, ms.ToArray());
                }
                // Publish event
                await _mediatr.Publish(new NewDocumentReceived(document, fileModel), token);
            }            
            return Ok(new { Success = false});
        }
    }
}
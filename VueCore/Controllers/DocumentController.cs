using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models;
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
            await _mediatr.Publish(new NewDocumentReceived(document), token);

            return Ok(new {Success = true});
        }
    }
}
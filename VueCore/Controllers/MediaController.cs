using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VueCore.Models;
using VueCore.Services.Commands;

namespace VueCore.Controllers
{
    public class MediaController : Controller
    {
        private readonly ILogger<MediaController> _logger;
        private readonly IMediator _mediatr;

        public MediaController(ILogger<MediaController> logger, IMediator mediatr)
        {
            _logger = logger;
            _mediatr = mediatr;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 150000000)]
        [RequestSizeLimit(150000000)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken token) 
        {
            string groupId = string.Empty;
            if(Request.Headers.ContainsKey("x-vuecore-groupid"))
            {
                groupId = Request.Headers["x-vuecore-groupid"].ToString();
            } 
            else 
            {
                return Unauthorized("x-vuecore-groupid not set");
            }

            if (file.Length > 0)
            {
                _logger.LogInformation("Create request");
                using(var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms, token);
                    ms.Position = 0;
                    var request = new MediaProcessRequest(groupId, file, ms.ToArray());
                    await _mediatr.Send(request, token);
                    return Ok(new { Success = true, groupId});
                }
            }
            return Ok(new {Success = false});
        }
 
         [HttpPost]
        public async Task<IActionResult> RemoveMedia([FromBody]MediaJob mediaJob, CancellationToken token) 
        {
            if(mediaJob == null)
            {
                return Ok(new {Success = false});
            }
            await _mediatr.Send(new MediaPruneRequest(mediaJob), token);
            return Ok(new {Success = true});
        }

    }
}
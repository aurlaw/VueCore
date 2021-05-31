using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        [RequestFormLimits(MultipartBodyLengthLimit = 73400320)]
        [RequestSizeLimit(73400320)]
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
            _logger.LogInformation("Create request");
            var request = new MediaProcessRequest(groupId, file);
            await _mediatr.Send(request, token);

            // if (file.Length > 0)
            // {
            //     using(var ms = new MemoryStream())
            //     {
            //         await file.CopyToAsync(ms, token);
            //         ms.Position = 0;
            //     }
            // }
            return Ok(new { Success = true, groupId});
        }
    }
}
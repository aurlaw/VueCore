using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VueCore.Models;
using VueCore.Services;
using VueCore.Services.Commands;

namespace VueCore.Controllers
{
    public class VisionController : Controller
    {
        private readonly ILogger<VisionController> _logger;
        private readonly IMediator _mediatr;

        public VisionController(ILogger<VisionController> logger, IMediator mediatr)
        {
            _logger = logger;
            _mediatr = mediatr;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 4000000)]
        [RequestSizeLimit(4000000)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken token) 
        {
            if (file.Length > 0)
            {
                using(var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms, token);
                    ms.Position = 0;

                    var result = await _mediatr.Send(new VisionProcessRequest(file.FileName, file.ContentType, ms), token);
                    return Ok(result);
                    // var url = await _storageService.SaveDocumentAsync(file.FileName, file.ContentType, ms, token);
                    // VisionAnalysis analysis = null;
                    // if(!string.IsNullOrEmpty(url))
                    // {
                    //     analysis = await _visionService.AnalyzeImageUrlAsync(url);
                    // }
                    // return Ok(new { Success = true, Url = url, analysis = analysis});
                }
            }
            return Ok(new { Success = false});
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUpload([FromBody]IEnumerable<string> files, CancellationToken token) 
        {
            await _mediatr.Send(new VisionPruneRequest(files), token);
            // if(files != null && files.Any()) 
            // {
            //     foreach(var fileName in files) 
            //     {
            //         await _storageService.DeleteDocumentAsync(fileName, token);
            //         // var fullFile = Path.Combine(uploads, fileName);
            //         // if(System.IO.File.Exists(fullFile))
            //         // {
            //         //     System.IO.File.Delete(fullFile);
            //         // }
            //     }
            // }
            return Ok(new {file = files});
        }
    }
}
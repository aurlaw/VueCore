using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VueCore.Models;
using VueCore.Services;

namespace VueCore.Controllers
{
    public class VisionController : Controller
    {
        private readonly ILogger<VisionController> _logger;
        private readonly IStorageService _storageService;

        public VisionController(ILogger<VisionController> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken token) 
        {
            if (file.Length > 0)
            {
                using(var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms, token);
                    ms.Position = 0;
                    var url = await _storageService.SaveDocumentAsync(file.FileName, file.ContentType, ms, token);
                    return Ok(new { Result = true, Url = url});
                }
            }
            return Ok(new { Result = false});
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUpload([FromBody]IEnumerable<string> files, CancellationToken token) 
        {
            if(files != null && files.Any()) 
            {
                foreach(var fileName in files) 
                {
                    await _storageService.DeleteDocumentAsync(fileName, token);
                    // var fullFile = Path.Combine(uploads, fileName);
                    // if(System.IO.File.Exists(fullFile))
                    // {
                    //     System.IO.File.Delete(fullFile);
                    // }
                }
            }
            return Ok(new {file = files});
        }
    }
}
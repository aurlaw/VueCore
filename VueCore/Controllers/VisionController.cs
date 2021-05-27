using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VueCore.Models;

namespace VueCore.Controllers
{
    public class VisionController : Controller
    {
        private readonly ILogger<VisionController> _logger;
        private readonly IWebHostEnvironment _environment;

        public VisionController(ILogger<VisionController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _environment = env;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file) 
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            return Ok(new { Result = true, Url = file.FileName});
        }

        [HttpPost]
        public IActionResult RemoveUpload([FromBody]IEnumerable<string> files) 
        {
            if(files != null && files.Any()) 
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                foreach(var fileName in files) 
                {
                    var fullFile = Path.Combine(uploads, fileName);
                    if(System.IO.File.Exists(fullFile))
                    {
                        System.IO.File.Delete(fullFile);
                    }
                }
            }
            return Ok(new {file = files});
        }

    }
}

using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VueCore.Models.Options;

namespace VueCore.Services
{
    public class VisionService : IVisionService
    {
        private readonly ILogger<VisionService> _logger;
        private readonly AzureVisionSettings _settings;

        public VisionService(ILogger<VisionService> logger, IConfiguration config)
        {
            _logger = logger;
            _settings = config.GetSection("AzureVision").Get<AzureVisionSettings>();
        }

        public Task<string> AnalyzeImageUrl(string imageUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}
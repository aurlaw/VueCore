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
using VueCore.Models;

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

        public async Task<VisionAnalysis> AnalyzeImageUrlAsync(string imageUrl, CancellationToken token)
        {
            var client = AuthenticateClient();

            _logger.LogInformation($"Analyze image with ep {_settings.Endpoint}");
            // analyze image
            _logger.LogInformation($"Analyze image {imageUrl}");
            var results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: GetVisualFeatureTypes(), cancellationToken:token);
            var data = JsonConvert.SerializeObject(results);
            // _logger.LogInformation(data);
            var analysis = new VisionAnalysis();
            // description
            foreach(var caption in results.Description.Captions)
            {
                analysis.Descriptions.Add(new VisionSummary(caption.Text, caption.Confidence));
            }
            // categories
            foreach(var category in results.Categories) 
            {
                analysis.Categories.Add(new VisionCategory(category.Name, category.Score));
            }
            // tags
            foreach(var tag in results.Tags)
            {
                analysis.Tags.Add(new VisionSummary(tag.Name, tag.Confidence));
            }
            // object detection
            foreach(var obj in results.Objects) 
            {
                var rect = new Models.BoundingRect(obj.Rectangle.X, obj.Rectangle.Y, obj.Rectangle.W, obj.Rectangle.H);
                analysis.Objects.Add(new VisionObject(obj.Confidence, obj.ObjectProperty, rect));
            }
            // color
            var vColor = new VisionColor(
                    results.Color.IsBWImg, 
                    results.Color.AccentColor,
                    results.Color.DominantColorBackground,
                    results.Color.DominantColorForeground,
                    results.Color.DominantColors);
            analysis.AddColor(vColor);
            _logger.LogInformation("returning analysis");
            return analysis;

        }

        public async Task<Stream> GenerateThumbnailAsync(string imageUrl, int? width = null, int? height = null,CancellationToken token = default)
        {
            var client = AuthenticateClient();
            _logger.LogInformation($"Generating thumbnail with ep {_settings.Endpoint}");
            var thW = width.HasValue ? width.Value : _settings.ThumbnailWidth;
            var thH = height.HasValue ? height.Value : _settings.ThumbnailHeight;
            _logger.LogInformation($"Generating thumbnail with  {imageUrl} {thW}x{thH}");

            var thumbnailResult = await client.GenerateThumbnailAsync(thW, thH, imageUrl, true, cancellationToken:token);
            return thumbnailResult;
        }

        private ComputerVisionClient AuthenticateClient()
        {
            var client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(_settings.SubscriptionKey)
            ) {
                Endpoint = _settings.Endpoint
            };
            return client;
        }

        private IList<VisualFeatureTypes?> GetVisualFeatureTypes()
        {
            return new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Color, VisualFeatureTypes.Objects
            };
        }
    }
}
/*
// Analyze the URL image 
ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);


    // Creating a list that defines the features to be extracted from the image. 

    List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
    {
        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
        VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
        VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
        VisualFeatureTypes.Objects
    };

*/
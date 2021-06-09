using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VueCore.Models;
using VueCore.Services.Commands;

namespace VueCore.Services.Handlers
{
    public class VisionProcessHandler : IRequestHandler<VisionProcessRequest, VisionResult>
    {

        private readonly ILogger<VisionProcessHandler> _logger;
        private readonly IStorageService _storageService;
        private readonly IVisionService _visionService;

        public VisionProcessHandler(ILogger<VisionProcessHandler> logger, IStorageService storageService, IVisionService visionService)
        {
            _logger = logger;
            _storageService = storageService;
            _visionService = visionService;
        }

        public async Task<VisionResult> Handle(VisionProcessRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Process request: {request.FileName}");

            var imgUrl = await _storageService.SaveDocumentAsync(request.FileName, request.ContentType, request.DataStream, cancellationToken);
            VisionAnalysis analysis = null;
            string thumbnailUrl = null;
            if(!string.IsNullOrEmpty(imgUrl))
            {
                analysis = await _visionService.AnalyzeImageUrlAsync(imgUrl, cancellationToken);
                thumbnailUrl = await GenerateThumbnailAsync(imgUrl, request.ContentType, cancellationToken);
            }
            return new VisionResult
            {
                Success = true,
                Url = imgUrl,
                Analysis = analysis,
                ThumbnailUrl = thumbnailUrl
            };
        }

        private async Task<string> GenerateThumbnailAsync(string imgUrl, string contentType, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Generating thumbnails...{imgUrl}");
            var dataStream = await _visionService.GenerateThumbnailAsync(imgUrl, token: cancellationToken);
            var fileName = Path.GetFileNameWithoutExtension(imgUrl);
            var ext = Path.GetExtension(imgUrl);
            var thumbnailName = $"{fileName}_thumb{ext}";
            _logger.LogInformation($"Generating thumbnails...{imgUrl} - {thumbnailName}");
            var thumbnail = await _storageService.SaveDocumentAsync(thumbnailName, contentType, dataStream, cancellationToken);
            return thumbnail;
        }
    }
}
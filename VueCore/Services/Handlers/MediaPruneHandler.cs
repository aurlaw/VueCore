using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VueCore.Services.Commands;

namespace VueCore.Services.Handlers
{
    public class MediaPruneHandler : AsyncRequestHandler<MediaPruneRequest>
    {
        private readonly ILogger<MediaPruneHandler> _logger;
        private readonly IMediaService _mediaService;

        public MediaPruneHandler(ILogger<MediaPruneHandler> logger, IMediaService mediaService)
        {
            _logger = logger;
            _mediaService = mediaService;
        }

        protected override async Task Handle(MediaPruneRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Process request: {request.Job?.JobName}");
            await _mediaService.PruneAsync(request.Job.JobName, request.Job.InputAssetName, request.Job.OutputAssetName,
                request.Job.LocatorName, request.Job.StopEndpoint);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using VueCore.Hubs;
using VueCore.Services.Commands;

namespace VueCore.Services.Handlers
{
    public class MediaProcessHandler : AsyncRequestHandler<MediaProcessRequest>
    {
        private readonly ILogger<MediaProcessHandler> _logger;
        // IHubContext is used to dependency injection purposes
        // IMediaProcessHubClient is used, so we dont use magic strings
        private readonly IHubContext<MediaProcessHub,IMediaProcessHubClient> _hubClient; 
        private readonly IMediaService _mediaService;

        public MediaProcessHandler(ILogger<MediaProcessHandler> logger,
        IHubContext<MediaProcessHub, IMediaProcessHubClient> hubContext, IMediaService mediaService)
        {
            _logger = logger;
            _hubClient = hubContext;
            _mediaService = mediaService;
        }

        protected override async Task Handle(MediaProcessRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Process request: {request.GroupId} with file {request.File?.FileName}");

            await _hubClient.Clients.Groups(request.GroupId).SendReceived(request.File?.FileName);
            // throw new System.NotImplementedException();
        }
    }
}

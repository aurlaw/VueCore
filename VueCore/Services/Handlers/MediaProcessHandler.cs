using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using VueCore.Hubs;
using VueCore.Services.Commands;
using VueCore.Services.Hosted;

namespace VueCore.Services.Handlers
{
    public class MediaProcessHandler : AsyncRequestHandler<MediaProcessRequest>
    {
        private readonly ILogger<MediaProcessHandler> _logger;
        // IHubContext is used to dependency injection purposes
        // IMediaProcessHubClient is used, so we dont use magic strings
        private readonly IHubContext<MediaProcessHub,IMediaProcessHubClient> _hubClient; 
        private readonly IMediaService _mediaService;
        private readonly IBackgroundTaskQueue _taskQueue;
        public MediaProcessHandler(ILogger<MediaProcessHandler> logger,
        IHubContext<MediaProcessHub, IMediaProcessHubClient> hubContext, 
        IBackgroundTaskQueue taskQueue, IMediaService mediaService)
        {
            _logger = logger;
            _hubClient = hubContext;
            _taskQueue = taskQueue;
            _mediaService = mediaService;
        }

        protected override async Task Handle(MediaProcessRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Process request: {request.GroupId} with file {request.File?.FileName}");

            await _hubClient.Clients.Groups(request.GroupId).SendUploaded(request.File?.FileName);

            // add to task queue
            await _taskQueue.QueueBackgroundWorkItemAsync(async token => {
                _logger.LogInformation("Queue started...");
                await SubmitMedia(request, cancellationToken);
                _logger.LogInformation("Queue Completed.");
            });
        }

        private async ValueTask SubmitMedia(MediaProcessRequest request, CancellationToken cancellationToken) 
        {
            _logger.LogInformation($"SubmitMedia:  file {request.File?.FileName}");
            try 
            {
                // await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                var result = await _mediaService.EncodeMediaAsync(request.File.FileName, request.Data);

                await _hubClient.Clients.Groups(request.GroupId).SendReceived(result);
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await _hubClient.Clients.Groups(request.GroupId).SendError(ex.Message);
            }
        }
    }
}

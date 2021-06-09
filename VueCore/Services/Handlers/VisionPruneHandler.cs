using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VueCore.Services.Commands;

namespace VueCore.Services.Handlers
{
    public class VisionPruneHandler : AsyncRequestHandler<VisionPruneRequest>
    {
        private readonly ILogger<VisionPruneHandler> _logger;
        private readonly IStorageService _storageService;

        public VisionPruneHandler(ILogger<VisionPruneHandler> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        protected override async Task Handle(VisionPruneRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Process request: {request.FileList.Count()}");
            if(request.FileList != null && request.FileList.Any())
            {
                foreach(var fileName in request.FileList) 
                {
                    await _storageService.DeleteDocumentAsync(fileName, cancellationToken);
                }
            }
        }
    }
}
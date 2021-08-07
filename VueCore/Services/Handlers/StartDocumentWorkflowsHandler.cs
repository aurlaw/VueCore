using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VueCore.Events;

namespace VueCore.Services.Handlers
{
    public class StartDocumentWorkflowsHandler : INotificationHandler<NewDocumentReceived>
    {
        private readonly IWorkflowRegistry _workflowRegistry;
        private readonly IWorkflowDefinitionDispatcher _workflowDispatcher;
        private readonly ILogger<StartDocumentWorkflowsHandler> _logger;
        private readonly IConfiguration _configuration;

        public StartDocumentWorkflowsHandler(IWorkflowRegistry workflowRegistry, IWorkflowDefinitionDispatcher workflowDispatcher, ILogger<StartDocumentWorkflowsHandler> logger, IConfiguration configuration)
        {
            _workflowRegistry = workflowRegistry;
            _workflowDispatcher = workflowDispatcher;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Handle(NewDocumentReceived notification, CancellationToken cancellationToken)
        {
            var document = notification.Document;
            var tagName = _configuration["Elsa:DocumentManager:TagName"];
            _logger.LogInformation($"Start workflow ({tagName}) for document: {document.Id}");

            // Get all workflow blueprints tagged with the received tag name.
            var workflowBlueprint = await _workflowRegistry.FindManyAsync(x => x.IsPublished && x.Tag == tagName, cancellationToken);

            // Dispatch each workflow. Each workflow will be correlated by Document ID.
            foreach(var workflow in workflowBlueprint)
            {
                var request = new ExecuteWorkflowDefinitionRequest(
                    workflow.Id,
                    CorrelationId: document.Id
                );
                await _workflowDispatcher.DispatchAsync(request, cancellationToken);
            }
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using VueCore.Events;

namespace VueCore.Services.Handlers
{
    public class StartDocumentWorkflowsHandler : INotificationHandler<NewDocumentReceived>
    {
        private readonly IWorkflowRegistry _workflowRegistry;
        private readonly IWorkflowDefinitionDispatcher _workflowDispatcher;
        private readonly ILogger<StartDocumentWorkflowsHandler> _logger;

        public StartDocumentWorkflowsHandler(IWorkflowRegistry workflowRegistry, IWorkflowDefinitionDispatcher workflowDispatcher, ILogger<StartDocumentWorkflowsHandler> logger)
        {
            _workflowRegistry = workflowRegistry;
            _workflowDispatcher = workflowDispatcher;
            _logger = logger;
        }

        public Task Handle(NewDocumentReceived notification, CancellationToken cancellationToken)
        {
            var document = notification.Document;
            _logger.LogInformation($"Start workflow for document: {document.Id}");

            return Task.CompletedTask;

        }
    }
}
/*
            // Get all workflow blueprints tagged with the received document type ID.
            var workflowBlueprints = await _workflowRegistry.FindManyAsync(x => x.IsPublished && x.Tag == documentTypeId, cancellationToken).ToList();

            // Dispatch each workflow. Each workflow will be correlated by Document ID.
            foreach (var workflowBlueprint in workflowBlueprints) 
                await _workflowDispatcher.DispatchAsync(new ExecuteWorkflowDefinitionRequest(workflowBlueprint.Id, CorrelationId: document.Id), cancellationToken);


*/
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VueCore.Events;

namespace VueCore.Services.Handlers
{
    public class StartPaymentWorkflowsHandler : INotificationHandler<SchedulePayment>
    {
        private readonly IWorkflowRegistry _workflowRegistry;
        private readonly IWorkflowDefinitionDispatcher _workflowDispatcher;
        private readonly ILogger<StartPaymentWorkflowsHandler> _logger;
        private readonly IConfiguration _configuration;

        public StartPaymentWorkflowsHandler(IWorkflowRegistry workflowRegistry, IWorkflowDefinitionDispatcher workflowDispatcher, ILogger<StartPaymentWorkflowsHandler> logger, IConfiguration configuration)
        {
            _workflowRegistry = workflowRegistry;
            _workflowDispatcher = workflowDispatcher;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Handle(SchedulePayment notification, CancellationToken cancellationToken)
        {

            var tagName = _configuration["Elsa:PaymentManager:TagName"];
            _logger.LogInformation($"Staring workflow ({tagName}) for payment: {notification.Payment.Id} and user: {notification.User.Email}");

            // Get all workflow blueprints tagged with the received tag name.
            var workflowBlueprint = await _workflowRegistry.FindManyAsync(x => x.IsPublished && x.Tag == tagName, cancellationToken);

            // Dispatch each workflow. Each workflow will be correlated by Document ID.
            foreach(var workflow in workflowBlueprint)
            {
                var request = new ExecuteWorkflowDefinitionRequest(
                    workflow.Id,
                    Input: notification
                );
                await _workflowDispatcher.DispatchAsync(request, cancellationToken);
            }
        }
    }
}
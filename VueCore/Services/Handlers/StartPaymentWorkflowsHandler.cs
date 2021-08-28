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

        public Task Handle(SchedulePayment notification, CancellationToken cancellationToken)
        {

            var tagName = _configuration["Elsa:PaymentManager:TagName"];
            _logger.LogInformation($"Staring workflow ({tagName}) for payment: {notification.Payment.Id} and user: {notification.User.Email}");
            return Task.CompletedTask;

        }
    }
}
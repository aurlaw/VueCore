using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models.Domain;
using VueCore.Services;

namespace VueCore.Models.Activities
{
    [Action(Category = "Payment Management", 
        Description = "Processes the payment")]
    public class PaymentProcessor : Activity
    {
        private readonly ILogger<PaymentProcessor> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentProcessor(ILogger<PaymentProcessor> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [ActivityInput(
            Label = "Payment ID",
            Hint = "The ID of the payment to process",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string PaymentId {get;set;} = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            _logger.LogInformation($"Processing for id {PaymentId}");
            var payment = await _paymentService.GetPaymentAsync(PaymentId, context.CancellationToken);
            _logger.LogInformation($"Processing amount {payment.Amount.ToString("c")}");

            await _paymentService.PaymentCompleteAsync(PaymentId, context.CancellationToken);

            return Done();
        }
    }
}
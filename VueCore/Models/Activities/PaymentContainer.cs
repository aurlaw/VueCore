using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models.Domain;

namespace VueCore.Models.Activities
{
    [Action(Category = "Payment Management", 
        Description = "Acts as a starting point for a scheduled payment")]
    public class PaymentContainer : Activity
    {
        
        [ActivityInput(
            Label = "Scheduled Payment",
            Hint = "The Scheduled payment model to load",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public SchedulePayment SchedulePayment {get;set;} = default!;

        [ActivityOutput(Hint = "The Payment")]
        public Payment OutputPayment {get;set;} = default!;

        [ActivityOutput(Hint = "The User")]
        public ApplicationUser OutputUser {get;set;} = default!;

        protected override IActivityExecutionResult OnExecute()
        {
            OutputPayment = SchedulePayment.Payment;
            OutputUser = SchedulePayment.User;
            return Done();
        }

    }
}
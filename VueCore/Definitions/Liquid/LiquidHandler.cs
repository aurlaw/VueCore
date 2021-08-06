using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.Liquid.Messages;
using MediatR;
using VueCore.Models;
using VueCore.Models.Domain;
using Fluid;
using VueCore.Models.Activities;

namespace VueCore.Definitions.Liquid
{
    public class LiquidHandler : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            notification.TemplateContext.Options.MemberAccessStrategy.Register<User>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<RegistrationModel>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<DocumentFile>();
            return Task.CompletedTask;
        }
    }
}
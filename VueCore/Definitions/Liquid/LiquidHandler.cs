using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.Liquid.Messages;
using MediatR;
using VueCore.Models;
using VueCore.Models.Domain;
using Fluid;

namespace VueCore.Definitions.Liquid
{
    public class LiquidHandler : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            notification.TemplateContext.Options.MemberAccessStrategy.Register<User>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<RegistrationModel>();
            return Task.CompletedTask;
        }
    }
}
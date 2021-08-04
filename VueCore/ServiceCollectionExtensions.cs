using Microsoft.Extensions.DependencyInjection;
using VueCore.Models.Activities;
using Elsa;
using Elsa.Scripting.Liquid.Services;
using VueCore.Definitions.JavaScript;

namespace VueCore
{
    public static class ServiceCollectionExtensions
    {
        public static ElsaOptionsBuilder AddUserActivities(this ElsaOptionsBuilder elsa)
        {
            return elsa
                .AddActivity<CreateUser>()
                .AddActivity<ActivateUser>();

        }

        public static IServiceCollection AddElsaDefinitions(this IServiceCollection services)
        {
             return services
                .AddNotificationHandlersFrom<LiquidHandler>()
                .AddJavaScriptTypeDefinitionProvider<WorkflowDefinitionProvider>();   
        }
    }
}
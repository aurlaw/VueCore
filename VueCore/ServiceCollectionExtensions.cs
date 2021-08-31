using Microsoft.Extensions.DependencyInjection;
using VueCore.Models.Activities;
using Elsa;
using Elsa.Scripting.Liquid.Services;
using VueCore.Definitions.JavaScript;
using VueCore.Services.Handlers;
using VueCore.Services;
using System;
using VueCore.Models.Options;

namespace VueCore
{
    public static class ServiceCollectionExtensions
    {
        public static ElsaOptionsBuilder AddUserActivities(this ElsaOptionsBuilder elsa)
        {
            return elsa
                .AddActivity<CreateUser>()
                .AddActivity<DeleteUser>()
                .AddActivity<ActivateUser>()
                .AddActivity<DocumentContainer>()
                .AddActivity<UploadFile>()
                .AddActivity<DocumentUpdateFile>()
                .AddActivity<DocumentExtractor>()
                .AddActivity<DocumentAddIndex>()
                .AddActivity<GetDocument>()
                .AddActivity<PaymentContainer>()
                .AddActivity<PaymentProcessor>()
                ;
        }

        public static IServiceCollection AddElsaDefinitions(this IServiceCollection services)
        {
            /*Note: AddNotificationHandlersFrom is a part of the Elsa library but uses MediatR under the hood
            since we are using MediatR and are registering all handlers at startup  - we dont need to call this directly
            - if so, we will create duplicate events
            */ 
             return services
                // .AddNotificationHandlersFrom<LiquidHandler>()
                // .AddNotificationHandlersFrom<StartDocumentWorkflowsHandler>()
                .AddJavaScriptTypeDefinitionProvider<WorkflowDefinitionProvider>();   
        }

        public static IServiceCollection AddSearch(this IServiceCollection services, Action<SearchOptions> configureOptions) 
        {
            services.Configure(configureOptions);

            return services.AddSingleton<ISearchService, SearchService>();
        }
    }
}
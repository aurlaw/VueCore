using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using VueCore.Models.Options;
using VueCore.Services;

namespace VueCore.Areas.Identity
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, Action<SmtpOptions> configureOptions) 
        {
            services.Configure(configureOptions);

            return services.AddTransient<IEmailSender, IdentityEmailSender>();
        }
        
    }
}
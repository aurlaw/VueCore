using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VueCore.Areas.Identity.Data;
using VueCore.Models.Domain;

[assembly: HostingStartup(typeof(VueCore.Areas.Identity.IdentityHostingStartup))]
namespace VueCore.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<VueCoreIdentityDbContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("Sqlite")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<VueCoreIdentityDbContext>();

                var elsaSection = context.Configuration.GetSection("Elsa");
                services.AddEmailService(elsaSection.GetSection("Smtp").Bind);

            });
        }
    }
}
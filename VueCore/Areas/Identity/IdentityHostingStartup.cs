using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VueCore.Areas.Identity.Data;

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

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<VueCoreIdentityDbContext>();
            });
        }
    }
}
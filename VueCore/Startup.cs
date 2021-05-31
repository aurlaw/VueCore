using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueCore.Hubs;
using VueCore.Models.Options;
using VueCore.Services;
using VueCore.Services.Security;

namespace VueCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // custom services
            services.AddSingleton<IStorageService, StorageService>();
            services.AddSingleton<IVisionService, VisionService>();
            services.AddSingleton<AzureMediaSettings>();
            services.AddSingleton<MediaAuthentication>();
            services.AddSingleton<IMediaService, MediaService>();

            // routing/controllers
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews();

            // add mediatR
            services.AddMediatR(typeof(Startup).Assembly);   
            // add signalR
            services.AddSignalR(); 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<MediaProcessHub>("/mediaprocessHub");
            });

        }
    }
}

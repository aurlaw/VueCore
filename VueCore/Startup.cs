using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using VueCore.Hubs;
using VueCore.Models.Options;
using VueCore.Services;
using VueCore.Services.Hosted;
using VueCore.Services.Security;
using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using VueCore.Data;
using Microsoft.EntityFrameworkCore;
using VueCore.Providers.WorkflowContexts;
using VueCore.Data.StartupTasks;
using Elsa.Runtime;

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
            // health checks
            services.AddHealthChecks()
                .AddPrivateMemoryHealthCheck(15, failureStatus: HealthStatus.Degraded)
                .AddProcessAllocatedMemoryHealthCheck(15, failureStatus: HealthStatus.Degraded)
                .AddVirtualMemorySizeHealthCheck(15, failureStatus: HealthStatus.Degraded)
                .AddAzureBlobStorage(Configuration["AzureStorage:ConnectionString"], 
                    Configuration["AzureStorage:Container"],
                    failureStatus: HealthStatus.Degraded,
                    tags: new string[] { "azure", "blobstorage" })
                .AddSignalRHub($"{Configuration["Health:SignalRUrl"]}/mediaprocessHub", "MediaProcessHub",
                    failureStatus: HealthStatus.Degraded,
                    tags: new string[] {"signalR"})
                .AddApplicationInsightsPublisher(); 

            services.AddHealthChecksUI()
                .AddInMemoryStorage();

            // elsa integration
            var elsaSection = Configuration.GetSection("Elsa");
            var sqliteConn = Configuration.GetConnectionString("Sqlite");

            services
                .AddDbContextFactory<BlogContext>(options => options.UseSqlite(sqliteConn, sql => 
                    sql.MigrationsAssembly(typeof(Startup).Assembly.FullName)))
                .AddCors(cors => cors.AddDefaultPolicy(policy => 
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()))
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite(sqliteConn))
                    .AddConsoleActivities()
                    .AddJavaScriptActivities()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddEmailActivities(elsaSection.GetSection("Smtp").Bind)
                    .AddQuartzTemporalActivities()
                    .AddWorkflowsFrom<Startup>()
                    .AddUserActivities()
                )
                .AddElsaDefinitions()
                .AddWorkflowContextProvider<BlogPostWorkflowContextProvider>()
                .AddStartupTask<RunBlogMigrations>();

            // custom services
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IStorageService, StorageService>();
            services.AddSingleton<IVisionService, VisionService>();
            services.AddSingleton<AzureMediaSettings>();
            services.AddSingleton<MediaAuthentication>();
            services.AddSingleton<IMediaService, MediaService>();
            services.AddSingleton<IBackgroundTaskQueue>(ctx => {
                if(!int.TryParse(Configuration["QueueCapacity"], out var queueCapacity))
                    queueCapacity = 100;
                return new BackgroundTaskQueue(queueCapacity);
            });
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IDocumentService, DocumentService>();
            services.AddSingleton<IPdfExtractor, PdfExtractor>();
            // search service
            services.AddSearch(Configuration.GetSection("Search").Bind);
            

            // routing/controllers
            services.AddElsaApiEndpoints();
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
            app.UseHttpActivities();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<MediaProcessHub>("/mediaprocessHub");
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });                
                endpoints.MapHealthChecksUI();
            });

        }
    }
}

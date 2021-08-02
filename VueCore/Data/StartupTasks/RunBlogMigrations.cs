using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Microsoft.EntityFrameworkCore;

namespace VueCore.Data.StartupTasks
{
    public class RunBlogMigrations : IStartupTask
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public RunBlogMigrations(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public int Order => 0;

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.Database.MigrateAsync(cancellationToken);
            await dbContext.DisposeAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using VueCore.Models.Domain;

namespace VueCore.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<BlogPost> BlogPosts {get;set;} = default!;
        public DbSet<User> Users {get;set;} = default!;
        public DbSet<Document> Documents {get;set;} = default!;
    }
}
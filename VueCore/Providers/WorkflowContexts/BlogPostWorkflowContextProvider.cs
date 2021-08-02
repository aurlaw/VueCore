using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Http.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.EntityFrameworkCore;
using VueCore.Data;
using VueCore.Models.Domain;

namespace VueCore.Providers.WorkflowContexts
{
    public class BlogPostWorkflowContextProvider : WorkflowContextRefresher<BlogPost>
    {
        private readonly IDbContextFactory<BlogContext> _blogContextFactory;
        public BlogPostWorkflowContextProvider(IDbContextFactory<BlogContext> blogContextFactory)
        {
            _blogContextFactory = blogContextFactory;
        }
        public override async ValueTask<BlogPost> LoadAsync(LoadWorkflowContext context, CancellationToken cancellationToken = default)
        {
            var blogPostId = context.ContextId;
            await using var dbContext = _blogContextFactory.CreateDbContext();
            return await dbContext.BlogPosts.AsQueryable().FirstOrDefaultAsync(x => x.Id == blogPostId, cancellationToken);
        }
        public override async ValueTask<string> SaveAsync(SaveWorkflowContext<BlogPost> context, CancellationToken cancellationToken = default)
        {
            var blogPost = context.Context;
            await using var dbContext = _blogContextFactory.CreateDbContext();
            var dbSet = dbContext.BlogPosts;
            if(blogPost == null) 
            {
                // new post, get from body
                blogPost = ((HttpRequestModel)context.WorkflowExecutionContext.Input!).GetBody<BlogPost>();
                // generate id
                blogPost.Id = Guid.NewGuid().ToString("N");
                // ensure published is false
                blogPost.IsPublished = false;
                // set context
                context.WorkflowExecutionContext.WorkflowContext = blogPost;
                context.WorkflowExecutionContext.ContextId = blogPost.Id;
                // add post to db
                await dbSet.AddAsync(blogPost, cancellationToken);
            } 
            else 
            {
                // existing post
                var blogPostId = blogPost.Id;
                var existingPost = await dbSet.AsQueryable()
                    .Where(x => x.Id == blogPostId)
                    .FirstAsync(cancellationToken);

                // update
                dbContext.Entry(existingPost).CurrentValues.SetValues(blogPost);    
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return blogPost.Id;
        }
    }
}
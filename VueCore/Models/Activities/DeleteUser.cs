using Elsa.Services;
using Elsa.Services.Models;
using Elsa.Attributes;
using Elsa.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VueCore.Data;
using Elsa;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using System;
using System.Linq;

namespace VueCore.Models.Activities
{
    [Activity(Category = "Users", 
        Description = "Delete a User", 
        Outcomes = new[] { OutcomeNames.Done })]
    public class DeleteUser : Activity
    {
        private readonly IDbContextFactory<BlogContext> _blogContextFactory;
        private readonly ILogger<DeleteUser> _logger;

        public DeleteUser(IDbContextFactory<BlogContext> blogContextFactory, ILogger<DeleteUser> logger)
        {
            _blogContextFactory = blogContextFactory;
            _logger = logger;
        }
        [ActivityInput(
            Label = "UserId",
            Hint = "Enter an expression that evaluates to the ID of the user to activate.",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string UserId { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            await using var dbContext = _blogContextFactory.CreateDbContext();
            var existingUser = await dbContext.Users.AsQueryable()
                .Where(u => u.Id == UserId)
                .FirstOrDefaultAsync(context.CancellationToken);
            if(existingUser != null) 
            {
                dbContext.Users.Remove(existingUser);
                await dbContext.SaveChangesAsync(context.CancellationToken);
            }
            return Done();
        }
    }
}
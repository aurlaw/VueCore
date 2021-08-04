using Elsa.Services;
using Elsa.Services.Models;
using Elsa.Attributes;
using Elsa.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VueCore.Data;
using VueCore.Services.Security;
using Elsa;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using VueCore.Models.Domain;
using System;
using System.Linq;

namespace VueCore.Models.Activities
{
    [Activity(Category = "Users", 
        Description = "Activate a User", 
        Outcomes = new[] { OutcomeNames.Done, "Not Found"  })]
    public class ActivateUser : Activity
    {
       private readonly IDbContextFactory<BlogContext> _blogContextFactory;
        private readonly ILogger<CreateUser> _logger;

        public ActivateUser(IDbContextFactory<BlogContext> blogContextFactory, ILogger<CreateUser> logger)
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
            if (existingUser == null)
                return Outcome("Not Found");
            
            existingUser.IsActive = true;
            dbContext.Update(existingUser);

            await dbContext.SaveChangesAsync(context.CancellationToken);

            return Done();
        }

    }
}
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

namespace VueCore.Models.Activities
{
    [Activity(Category = "Users", 
        Description = "Create a User", 
        Outcomes = new[] { OutcomeNames.Done })]
    public class CreateUser : Activity
    {
        private readonly IDbContextFactory<BlogContext> _blogContextFactory;
        private readonly ILogger<CreateUser> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUser(IDbContextFactory<BlogContext> blogContextFactory, ILogger<CreateUser> logger, IPasswordHasher passwordHasher)
        {
            _blogContextFactory = blogContextFactory;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }
        
        [ActivityInput(
            Label = "UserName",
            Hint = "Enter an expression that evaluates to the name of the user to create.",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string UserName { get; set; } = default!;

        [ActivityInput(
            Label = "Email",
            Hint = "Enter an expression that evaluates to the email address of the user to create.",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string Email { get; set; } = default!;

        [ActivityInput(
            Label = "Password",
            Hint = "Enter an expression that evaluates to the password of the user to create.",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string Password { get; set; } = default!;

        [ActivityOutput] 
        public User Output {get; set;} = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var hashedPassword = _passwordHasher.HashPassword(Password);
            var user = new User 
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = UserName,
                Email = Email,
                Password = hashedPassword.Hashed,
                PasswordSalt = hashedPassword.Salt,
                IsActive = false
            };
            await using var dbContext = _blogContextFactory.CreateDbContext();
            var dbSet = dbContext.BlogPosts;
            // add user to db
            await dbContext.Users.AddAsync(user, context.CancellationToken);
            await dbContext.SaveChangesAsync(context.CancellationToken);
            Output = user;
            return Done();
        }
    }
}
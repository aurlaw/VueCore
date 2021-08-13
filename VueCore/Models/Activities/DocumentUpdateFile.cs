using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models.Domain;
using VueCore.Services;

namespace VueCore.Models.Activities
{
    [Action(Category = "Document Management", 
        Description = "Updates the File Url for a Document")]
    public class DocumentUpdateFile : Activity
    {
        private readonly ILogger<DocumentUpdateFile> _logger;
        private readonly IDocumentService _docService;

        public DocumentUpdateFile(ILogger<DocumentUpdateFile> logger, IDocumentService docService)
        {
            _logger = logger;
            _docService = docService;
        }
        [ActivityInput(
            Label = "Document ID",
            Hint = "The ID of the document to update",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string DocumentId { get; set; } = default!;
        [ActivityInput(
            Label = "File Url",
            Hint = "the File Url to update for the document",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string FileUrl { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            _logger.LogInformation($"Update {DocumentId} with url ({FileUrl})");

            await _docService.UpdateWithFileAsync(DocumentId, FileUrl, context.CancellationToken);

            return Done();
        }
    }
}
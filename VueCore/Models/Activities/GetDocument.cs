using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa.Providers.WorkflowStorage;

using Microsoft.Extensions.Logging;
using VueCore.Models.Domain;
using VueCore.Services;

namespace VueCore.Models.Activities
{
    public record DocumentFile(Document Document);

    [Action(Category = "Document Management", 
        Description = "Gets the specified document from the database.")]
    public class GetDocument : Activity
    {
        private readonly ILogger<GetDocument> _logger;
        private readonly IDocumentService _docService;

        public GetDocument(ILogger<GetDocument> logger, IDocumentService docService)
        {
            _logger = logger;
            _docService = docService;
        }
        [ActivityInput(
            Label = "Document ID",
            Hint = "The ID of the document to load",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string DocumentId { get; set; } = default!;

        [ActivityOutput(Hint = "The document")]
        public DocumentFile Output { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var document = await _docService.GetAsync(DocumentId, context.CancellationToken);
            Output = new DocumentFile(document);
            return Done();
        }

    }
}
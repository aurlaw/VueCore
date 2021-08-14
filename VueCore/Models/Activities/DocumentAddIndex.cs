using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models.Domain;
using VueCore.Models.Search;
using VueCore.Services;
namespace VueCore.Models.Activities
{
     [Action(Category = "Document Management", 
        Description = "Saves document and file data to index")]
    public class DocumentAddIndex : Activity
    {
       private readonly ILogger<DocumentAddIndex> _logger;
       private readonly IDocumentService _docService;
       private readonly ISearchService _searchService;

        public DocumentAddIndex(ILogger<DocumentAddIndex> logger, IDocumentService docService, ISearchService searchService)
        {
            _logger = logger;
            _docService = docService;
            _searchService = searchService;
        }

        [ActivityInput(
            Label = "Document ID",
            Hint = "The ID of the document to add",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string DocumentId { get; set; } = default!;

        [ActivityInput(
            Label = "File Content",
            Hint = "The text content of an uploaded document file",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string FileContent {get;set;} = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var document = await _docService.GetAsync(DocumentId, context.CancellationToken);
            _logger.LogInformation($"Using document: {document.Name}");
            var content = string.Concat(FileContent, " ", document.Notes);
            var searchDocument = new SearchDocument(document.Id, document.Name, content, document.UpdatedAt);
            await _searchService.SaveDocumentAsync(searchDocument, context.CancellationToken);

            return Done();
        }


    }
}

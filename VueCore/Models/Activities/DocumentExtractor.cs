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
        Description = "Extracts content from PDF and saves to index")]
   public class DocumentExtractor : Activity
    {
       private readonly ILogger<DocumentExtractor> _logger;
       private readonly IPdfExtractor _extractor;
       private readonly IDocumentService _docService;
       private readonly ISearchService _searchService;

        public DocumentExtractor(ILogger<DocumentExtractor> logger, IPdfExtractor extractor, IDocumentService docService, ISearchService searchService)
        {
            _logger = logger;
            _extractor = extractor;
            _docService = docService;
            _searchService = searchService;
        }
        [ActivityInput(
            Label = "Document ID",
            Hint = "The ID of the document to update",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public string DocumentId { get; set; } = default!;

        [ActivityInput(
            Label = "File  to upload",
            Hint = "Represents a FileModel object",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public FileModel File {get;set;} = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            _logger.LogInformation($"Using file: {File.FileName}");
            using var ms = new MemoryStream(File.Content);
            var pdfText = await _extractor.GetAsText(ms, context.CancellationToken);
            _logger.LogInformation("Extracted Content");
            _logger.LogInformation(pdfText);
            _logger.LogInformation("-------------------");
            _logger.LogInformation("Save to index");
            await AddToSearchIndex(pdfText, context.CancellationToken);
            return Done();
        }

        private async Task AddToSearchIndex(string pdfContent, CancellationToken cancellationToken = default) 
        {
            var document = await _docService.GetAsync(DocumentId, cancellationToken);
            _logger.LogInformation($"Using document: {document.Name}");
            var content = string.Concat(pdfContent, " ", document.Notes);
            var searchDocument = new SearchDocument(document.Id, document.Name, content, document.UpdatedAt);
            await _searchService.SaveDocumentAsync(searchDocument, cancellationToken);

        }
    }
}

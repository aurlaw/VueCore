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
        Description = "Extracts content from PDF")]
   public class DocumentExtractor : Activity
    {
       private readonly ILogger<DocumentExtractor> _logger;
       private readonly IPdfExtractor _extractor;
       private readonly IDocumentService _docService;

        public DocumentExtractor(ILogger<DocumentExtractor> logger, IPdfExtractor extractor, IDocumentService docService)
        {
            _logger = logger;
            _extractor = extractor;
            _docService = docService;
        }

        [ActivityInput(
            Label = "File  to upload",
            Hint = "Represents a FileModel object",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public FileModel File {get;set;} = default!;

        [ActivityOutput(Hint = "The Extracted Text")]
        public string Output { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            _logger.LogInformation($"Using file: {File.FileName}");
            using var ms = new MemoryStream(File.Content);
            var pdfText = await _extractor.GetAsText(ms, context.CancellationToken);
            _logger.LogInformation("Extracted Content");
            _logger.LogInformation(pdfText);
            _logger.LogInformation("-------------------");
            Output = pdfText;
            return Done();
        }

    }
}

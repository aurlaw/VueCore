using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models.Domain;

namespace VueCore.Models.Activities
{
    [Action(Category = "Document Management", 
        Description = "Acts as a starting point for a Document and FileModel")]
    public class DocumentContainer : Activity
    {
        private readonly ILogger<DocumentContainer> _logger;

        public DocumentContainer(ILogger<DocumentContainer> logger)
        {
            _logger = logger;
        }

        [ActivityInput(
            Label = "Document Received",
            Hint = "The document/file to load",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public NewDocumentReceived DocumentReceived { get; set; } = default!;


        [ActivityOutput(Hint = "The document")]
        public Document OutputDocument { get; set; } = default!;

        [ActivityOutput(Hint = "The File Model")]
        public FileModel OutputFile { get; set; } = default!;


        protected override IActivityExecutionResult OnExecute()
        {
            OutputDocument = DocumentReceived.Document;
            OutputFile = DocumentReceived.File;

            return Done();
        }

    }
}
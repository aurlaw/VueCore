using System;
using Microsoft.Extensions.Logging;
using VueCore.Services;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using VueCore.Models.Domain;
using System.IO;

namespace VueCore.Models.Activities
{
    [Action(Category = "Storage", 
        Description = "Uploads file to storage container")]
    public class UploadFile : Activity
    {
        private readonly ILogger<UploadFile> _logger;
        private readonly IStorageService _storageService;

        public UploadFile(ILogger<UploadFile> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        [ActivityInput(
            Label = "File  to upload",
            Hint = "Represents a FileModel object",
            SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid}
        )]
        public FileModel File {get;set;} = default!;

        [ActivityOutput(Hint = "The absolute URL of the uploaded file")]
        public string OutputDocumentUrl {get;set;} = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            _logger.LogInformation($"Uploading file: {File.FileName}");
            using var ms = new MemoryStream(File.Content);
            var uploadUrl = await _storageService.SaveDocumentAsync(File.FileName, File.MimeType, ms, context.CancellationToken);
            _logger.LogInformation($"Uploaded url: {uploadUrl}");

            OutputDocumentUrl = uploadUrl;

            return Done();
        }

    }
}

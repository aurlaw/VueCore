using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BitMiracle.Docotic.Pdf;
using Microsoft.Extensions.Logging;

namespace VueCore.Services
{
    public class PdfExtractor : IPdfExtractor
    {
        private readonly ILogger<PdfExtractor> _logger;

        public PdfExtractor(ILogger<PdfExtractor> logger)
        {
            _logger = logger;
        }

        public Task<string> GetAsText(Stream fileStream, CancellationToken cancellationToken = default)
        {
            try 
            {
                using(var pdf = new PdfDocument(fileStream)) 
                {
                    var options = new PdfTextExtractionOptions
                    {
                        SkipInvisibleText = true,
                        WithFormatting = false
                    };
                    return Task.FromResult(pdf.GetText());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                    return Task.FromResult(string.Empty);
            }
        }
    }
}
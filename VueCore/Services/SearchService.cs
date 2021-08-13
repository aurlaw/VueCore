using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VueCore.Models.Search;

namespace VueCore.Services
{
    public class SearchService : ISearchService
    {
        // Ensures index backward compatibility
        const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        private readonly ILogger<SearchService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        private string _indexPath;
        private IndexWriterConfig _indexConfig;
        public SearchService(ILogger<SearchService> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;

            Configure();
        }

        private void Configure() 
        {
            // Construct a machine-independent path for the index
            var indexName = _configuration["Elsa:DocumentManager:SearchIndex"];
            _indexPath = Path.Combine(_environment.WebRootPath, indexName);
            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);    
            // Create an index writer
            _indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            
        }
        public Task SaveDocumentsAsync(IList<SearchDocument> documentList, CancellationToken cancellationToken = default)
        {
            if(documentList.Any())
            {
                // create writer
                using var dir = FSDirectory.Open(_indexPath);
                using var writer = new IndexWriter(dir, _indexConfig);

                foreach(var document in documentList)
                {
                    var doc = new Document
                    {
                        // StringField indexes but doesn't tokenize
                        new StringField("id", document.Id, Field.Store.YES),
                        new StringField("name", document.Name, Field.Store.YES),
                        new TextField("content", document.Content, Field.Store.YES),
                        new TextField("updated", document.UpdateAd.ToString("s"), Field.Store.YES)
                    };
                    writer.AddDocument(doc);
                }
                writer.Flush(triggerMerge:false, applyAllDeletes: false);
            }
            return Task.CompletedTask;
        }

        public  Task SaveDocumentAsync(SearchDocument document, CancellationToken cancellationToken = default)
        {
            var list = new List<SearchDocument>{document};
            return SaveDocumentsAsync(list, cancellationToken);
        }
    }
}
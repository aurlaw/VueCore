using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VueCore.Models.Options;
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
        private readonly SearchOptions _options;

        private string _indexPath;
        // private IndexWriterConfig _indexConfig;
        private StandardAnalyzer _analyzer;
        public SearchService(ILogger<SearchService> logger, IConfiguration configuration, IWebHostEnvironment environment, IOptions<SearchOptions> options)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _options = options.Value;

            Configure();
        }

        private void Configure() 
        {
            // Construct a machine-independent path for the index
            _indexPath = Path.Combine(_environment.WebRootPath, _options.IndexName);
            // Create an analyzer to process the text
            _analyzer = new StandardAnalyzer(AppLuceneVersion);    
            
        }
        public Task SaveDocumentsAsync(IList<SearchDocument> documentList, CancellationToken cancellationToken = default)
        {
            if(documentList.Any())
            {
                // create writer
                using var dir = FSDirectory.Open(_indexPath);
                var indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
                // Create an index writer
                using var writer = new IndexWriter(dir, indexConfig);

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

        public Task<IEnumerable<SearchDocument>> SearchAsync(string term, CancellationToken cancellationToken = default)
        {
            var list = new List<SearchDocument>();
            if(!string.IsNullOrEmpty(term))
            {
                // create index writer
                using var dir = FSDirectory.Open(_indexPath);
                var indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
                // Create an index writer
                using var writer = new IndexWriter(dir, indexConfig);
                // get reader
                using var reader = writer.GetReader(applyAllDeletes: true);
                var searcher = new IndexSearcher(reader);
                // Search with query
                var parser = new MultiFieldQueryParser(AppLuceneVersion, new[] { "field1", "field2" }, 
                    _analyzer);
//var query = parser.GetFuzzyQuery("fieldName", "featured", 0.7f);
                // var query = new TermQuery(new Term("content", term));
                var query = parser.CreatePhraseQuery("content", term);
                // get hits
                var hits = searcher.Search(query, 50).ScoreDocs;
                foreach(var hit in hits)
                {
                    var doc = searcher.Doc(hit.Doc);
                    list.Add(Convert(doc));
                }
            }

            return Task.FromResult((IEnumerable<SearchDocument>)list);
        }

        private SearchDocument Convert(Document doc) 
        {
            var id = doc.Get("id");
            var name = doc.Get("name");
            var content = doc.Get("content");
            var updated = doc.Get("updated");
            return new SearchDocument(id, name, content, DateTime.Parse(updated));
        }
    }
}
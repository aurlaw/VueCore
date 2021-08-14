using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VueCore.Models.Search;

namespace VueCore.Services
{
    public interface ISearchService
    {
         Task SaveDocumentAsync(SearchDocument document, CancellationToken cancellationToken = default);
         Task SaveDocumentsAsync(IList<SearchDocument> documentList, CancellationToken cancellationToken = default);
         Task<IEnumerable<SearchDocument>> SearchAsync(string term, CancellationToken cancellationToken = default );
    }
}
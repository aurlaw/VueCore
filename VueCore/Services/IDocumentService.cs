using System.Threading;
using System.Threading.Tasks;
using VueCore.Models.Domain;

namespace VueCore.Services
{
    public interface IDocumentService
    {
         Task<Document> SaveDocumentAsync(string name, string notes, CancellationToken cancellationToken = default);
         Task<Document> GetAsync(string id, CancellationToken cancellationToken = default);
         Task UpdateAsync(string id, string name, string notes, string fileUrl, CancellationToken cancellationToken = default);
         Task UpdateWithFileAsync(string id, string fileUrl, CancellationToken cancellationToken = default);
    }
}
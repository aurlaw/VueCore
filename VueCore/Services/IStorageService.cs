using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VueCore.Services
{
    public interface IStorageService
    {
        Task<string> SaveDocumentAsync(string fileName, string contentType, Stream fileStream, CancellationToken token);
        Task<bool> DeleteDocumentAsync(string fileUrl, CancellationToken token);         
    }
}
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VueCore.Services
{
    public interface IPdfExtractor
    {
         Task<string> GetAsText(Stream fileStream, CancellationToken cancellationToken = default);
    }
}
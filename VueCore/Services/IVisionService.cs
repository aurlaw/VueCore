using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VueCore.Models;

namespace VueCore.Services
{
    public interface IVisionService
    {
         Task<VisionAnalysis> AnalyzeImageUrlAsync(string imageUrl, CancellationToken token);

         Task<Stream> GenerateThumbnailAsync(string imageUrl, int? width = null, int? height = null,CancellationToken token = default);
    }
}
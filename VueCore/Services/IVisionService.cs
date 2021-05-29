using System.Threading.Tasks;
using VueCore.Models;

namespace VueCore.Services
{
    public interface IVisionService
    {
         Task<VisionAnalysis> AnalyzeImageUrlAsync(string imageUrl);
    }
}
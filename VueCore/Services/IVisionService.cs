using System.Threading.Tasks;

namespace VueCore.Services
{
    public interface IVisionService
    {
         Task<string> AnalyzeImageUrl(string imageUrl);
    }
}
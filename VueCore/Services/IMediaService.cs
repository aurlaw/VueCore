using System.Threading.Tasks;

namespace VueCore.Services
{
    public interface IMediaService
    {
         Task<string> EncodeMedia(string assetName, byte[] assetData);
    }
}
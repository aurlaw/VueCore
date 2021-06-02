using System.Threading.Tasks;

namespace VueCore.Services
{
    public interface IMediaService
    {
         Task<string> EncodeMediaAsync(string assetName, byte[] assetData);
    }
}
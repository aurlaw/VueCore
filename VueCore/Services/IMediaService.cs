using System;
using System.Threading;
using System.Threading.Tasks;
using VueCore.Models;

namespace VueCore.Services
{
    public interface IMediaService
    {

         Task<Tuple<bool, MediaJob, MediaException>> EncodeMediaAsync(
            string title,
            string assetName, 
            byte[] assetData, 
            Action<string> progess, 
            bool downloadAssets,
            CancellationToken token);

         Task PruneAsync(string jobName, string inputAssetName, string outputAssetName, string streamingLocatorName,
            bool stopEndpoint);
    }
}
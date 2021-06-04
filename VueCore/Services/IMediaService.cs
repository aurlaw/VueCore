using System;
using System.Threading;
using System.Threading.Tasks;
using VueCore.Models;

namespace VueCore.Services
{
    public interface IMediaService
    {
         Task<Tuple<bool, MediaJob, MediaException>> EncodeMediaAsync(string assetName, byte[] assetData, Action<string> progess, CancellationToken token);
    }
}
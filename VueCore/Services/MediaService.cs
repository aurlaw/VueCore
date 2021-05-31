using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VueCore.Models.Options;
using VueCore.Services.Security;

namespace VueCore.Services
{
    public class MediaService : IMediaService
    {
        private readonly ILogger<MediaService> _logger;
        private readonly AzureMediaSettings _settings;
        private readonly MediaAuthentication _authentication;
        

        public Task<string> EncodeMedia(string assetName, byte[] assetData)
        {
            throw new System.NotImplementedException();
        }
    }
}
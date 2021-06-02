using System.Threading.Tasks;
using Microsoft.Azure.Management.Media;
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

        public MediaService(ILogger<MediaService> logger, AzureMediaSettings settings, MediaAuthentication authentication)
        {
            _logger = logger;
            _settings = settings;
            _authentication = authentication;
        }

        public async Task<string> EncodeMediaAsync(string assetName, byte[] assetData)
        {
            _logger.LogInformation($"EncodeMediaAsync: {assetName} with file: {assetData.Length}");

            IAzureMediaServicesClient client = await _authentication.CreateMediaServicesClientAsync(_settings, false);
            //TODO:

            return client.BaseUri.AbsoluteUri;
        }
    }
}

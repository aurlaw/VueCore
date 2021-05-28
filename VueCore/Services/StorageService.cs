using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VueCore.Models.Options;

namespace VueCore.Services
{
    public class StorageService : IStorageService
    {
        private readonly ILogger<StorageService> _logger;
        private readonly AzureStorageSettings _settings;

        public StorageService(ILogger<StorageService> logger, IConfiguration config)
        {
            _logger = logger;
            _settings = config.GetSection("AzureStorage").Get<AzureStorageSettings>();
        }

        public async Task<bool> DeleteDocumentAsync(string fileUrl, CancellationToken token)
        {
            _logger.LogInformation($"fileUrl: {fileUrl}");
            if(string.IsNullOrEmpty(fileUrl))
            {
                throw new ArgumentException(nameof(fileUrl));
            }
            var blobContainer = await GetContainerAsync();
            var fileName = Path.GetFileName(fileUrl);
            // Get a reference to a blob
            var blobClient = blobContainer.GetBlobClient(fileName);
            return await blobClient.DeleteIfExistsAsync(cancellationToken: token);
        }

        public async Task<string> SaveDocumentAsync(string fileName, string contentType, Stream fileStream, CancellationToken token)
        {
            _logger.LogInformation($"fileName: {fileName}");
            var blobContainer = await GetContainerAsync();
            // Get a reference to a blob
            var blobClient = blobContainer.GetBlobClient(fileName);
            var options = new BlobUploadOptions();
            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = contentType;
            options.HttpHeaders = blobHttpHeaders;

            options.HttpHeaders.ContentType = contentType;

            await blobClient.UploadAsync(fileStream, options, token);

            return string.Concat(blobClient.Uri.AbsoluteUri, "?", DateTime.UtcNow.ToEpoch());
        }

        private Task<BlobContainerClient> GetContainerAsync()
        {
            _logger.LogInformation($"connection: {_settings.ConnectionString}");
            _logger.LogInformation($"container: {_settings.Container}");
            var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            return Task.FromResult(blobServiceClient.GetBlobContainerClient(_settings.Container));
        }
    }
}
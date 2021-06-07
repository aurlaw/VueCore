using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;
using VueCore.Models.Options;
using VueCore.Services.Security;
using VueCore.Models;
using System.Collections.Generic;

namespace VueCore.Services
{
    public class MediaService : IMediaService
    {

        private const string OutputFolder = @"MediaOutput";
        // private const string CustomTransform = "Custom_H264_3Layer";
        private const string DefaultTransform = "H624Multi720p";
        private const string DefaultStreamingEndpointName = "default";   // Change this to your Endpoint name.

        private readonly ILogger<MediaService> _logger;
        private readonly AzureMediaSettings _settings;
        private readonly MediaAuthentication _authentication;

        private readonly IWebHostEnvironment _environment;

        public MediaService(ILogger<MediaService> logger, AzureMediaSettings settings, MediaAuthentication authentication, IWebHostEnvironment environment)
        {
            _logger = logger;
            _settings = settings;
            _authentication = authentication;
            _environment = environment;
        }


        public async Task PruneAsync(string jobName, string inputAssetName, string outputAssetName, string streamingLocatorName,
            bool stopEndpoint)
        {

            _logger.LogInformation($"PruneAsync: {jobName}");
            IAzureMediaServicesClient client = await GetClient();

            await CleanUpAsync(client, _settings.ResourceGroup, _settings.AccountName, DefaultTransform, jobName, inputAssetName, outputAssetName, 
                streamingLocatorName, stopEndpoint, DefaultStreamingEndpointName);
        }

        public async Task<Tuple<bool, MediaJob, MediaException>>  EncodeMediaAsync(
            string title,
            string assetName, 
            byte[] assetData, 
            Action<string> progess, 
            bool downloadAssets,
            CancellationToken token)
        {
            
            _logger.LogInformation($"EncodeMediaAsync: {assetName} with file: {assetData.Length}");
            bool isSuccess = false;
            MediaJob result = null;
            MediaException exception = null;

            IAzureMediaServicesClient client = await GetClient();
            
            // create unique name to prevent collisions with dup file names
            string uniqueness = Guid.NewGuid().ToString().Substring(0, 13);
            string jobName = $"job-{uniqueness}";
            string locatorName = $"locator-{uniqueness}";
            string inputAssetName = $"input-{assetName}{uniqueness}";
            string outputAssetName = $"output-{assetName}{uniqueness}";
            bool stopEndpoint = false;            



            try
            {
                // Ensure that you have customized encoding Transform.  This is really a one time setup operation.
                progess("Creating Transform...");
                // var transform = await CreateCustomTransform(client, _settings.ResourceGroup, _settings.AccountName, CustomTransform);
                var transform = await CreateBuiltinTransform(client, _settings.ResourceGroup, _settings.AccountName, DefaultTransform);
                _logger.LogInformation($"Transform created...{transform.Description}");

                // Create a new input Asset and upload the specified local video file into it.
                progess("Creating input Asset...");
                var inputAsset = await CreateInputAssetAsync(client, _settings.ResourceGroup, _settings.AccountName, 
                    inputAssetName, assetName, assetData, token);
                _logger.LogInformation($"Input Asset created...{inputAsset.AssetId}");
                
                // Output from the Job must be written to an Asset, so let's create one
                progess("Creating output Asset...");
                var outputAsset = await CreateOutputAssetAsync(client, _settings.ResourceGroup, _settings.AccountName, outputAssetName, token);                
                _logger.LogInformation($"Output Asset created...{outputAsset.AssetId}");

                // create job
                progess("Creating and Submitting Job...");
                var job = await SubmitJobAsync(client, _settings.ResourceGroup, _settings.AccountName, DefaultTransform, jobName, inputAsset.Name, outputAsset.Name, token);                
                _logger.LogInformation($"Job created...{job.Name}");

                 DateTime startedTime = DateTime.Now;

                 //TODO: event hub
                // Polling is not a recommended best practice for production applications because of the latency it introduces.
                // Overuse of this API may trigger throttling. Developers should instead use Event Grid and listen for the status events on the jobs
                _logger.LogInformation("Polling job status...");
                progess("Polling job status...");
                job = await WaitForJobToFinishAsync(client, _settings.ResourceGroup, _settings.AccountName, DefaultTransform, jobName, progess, token);                 
                TimeSpan elapsed = DateTime.Now - startedTime;     
                if (job.State == JobState.Finished)
                {
                    _logger.LogInformation($"Job finished: {elapsed}");
                    progess($"Job finished: {elapsed}");
                    var thumbnailList = await GenerateResults(client, _settings.ResourceGroup, _settings.AccountName, outputAsset.Name, OutputFolder,  progess, downloadAssets, token);

                    //Streaming
                    progess("Creating Stream endpoints...");
                    var locator = await CreateStreamingLocatorAsync(client, _settings.ResourceGroup, _settings.AccountName, outputAssetName, locatorName, token);
                    var streamingEndpoint = await client.StreamingEndpoints.GetAsync(_settings.ResourceGroup, _settings.AccountName, DefaultStreamingEndpointName, token);
                    if(streamingEndpoint != null)
                    {
                        if(streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
                        {
                            _logger.LogInformation("Streaming Endpoint was Stopped, restarting now..");
                            progess("Streaming Endpoint was Stopped, restarting now..");
                            await client.StreamingEndpoints.StartAsync(_settings.ResourceGroup, _settings.AccountName, DefaultStreamingEndpointName, token);
                            // Since we started the endpoint, we should stop it in cleanup.
                            stopEndpoint = true;
                        }
                    }    
                    _logger.LogInformation("Getting the Streaming manifest URLs for HLS and DASH:");
                    progess("Getting the Streaming manifest URLs for HLS and DASH:");
                    var streamUrls = await GetStreamingUrlsAsync(client, _settings.ResourceGroup, _settings.AccountName, locator.Name, streamingEndpoint, token);
                    _logger.LogInformation($"Retuning {streamUrls.Count} URLs");

                    isSuccess = true;
                    result = new MediaJob(jobName, locatorName, inputAssetName, outputAssetName, 
                        stopEndpoint, streamUrls, thumbnailList.FirstOrDefault(), title);
                }
                else if (job.State == JobState.Error)
                {
                    _logger.LogInformation($"ERROR: Job finished with error message: {job.Outputs[0].Error.Message}");
                    _logger.LogInformation($"ERROR: error details: {job.Outputs[0].Error.Details[0].Message}");
                    exception =  new MediaException(job.Outputs[0].Error.Message);
                    _logger.LogInformation("Cleaning up...");
                    await CleanUpAsync(client, _settings.ResourceGroup, _settings.AccountName, DefaultTransform, jobName, 
                        inputAssetName, outputAssetName, locatorName, stopEndpoint, DefaultStreamingEndpointName);

                }
            }
            catch (ApiErrorException e)
            {
                _logger.LogError(e, e.Message);
                exception =  new MediaException(e.Message, e);
                _logger.LogInformation("Cleaning up...");
                await CleanUpAsync(client, _settings.ResourceGroup, _settings.AccountName, DefaultTransform, jobName, 
                    inputAssetName, outputAssetName, locatorName, stopEndpoint, DefaultStreamingEndpointName);

            }
            return new Tuple<bool, MediaJob, MediaException>(isSuccess, result, exception);
        }


        /// <summary>
        /// Authenticates and returns Media Services Client
        /// </summary>
        /// <returns></returns>
        private async Task<IAzureMediaServicesClient> GetClient() 
        {
            var client = await _authentication.CreateMediaServicesClientAsync(_settings, false);
            // Set the polling interval for long running operations to 2 seconds.
            // The default value is 30 seconds for the .NET client SDK
            client.LongRunningOperationRetryTimeout = 2;
            return client;
        }

        /// <summary>
        /// If the specified transform exists, return that transform. If the it does not
        /// exist, creates a new transform with the specified output. In this case, the
        /// output is set to encode a video using a custom preset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The transform name.</param>
        /// <returns></returns>
        private async Task<Transform> CreateCustomTransform(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string transformName)
        {
            // Does a transform already exist with the desired name? Assume that an existing Transform with the desired name
            // also uses the same recipe or Preset for processing content.
            Transform transform = client.Transforms.Get(resourceGroupName, accountName, transformName);

            if (transform == null)
            {
                _logger.LogInformation("Creating a custom transform...");
                // Create a new Transform Outputs array - this defines the set of outputs for the Transform
                TransformOutput[] outputs = new TransformOutput[]
                {
                    // Create a new TransformOutput with a custom Standard Encoder Preset
                    // This demonstrates how to create custom codec and layer output settings
                    

                  new TransformOutput(
                        new StandardEncoderPreset(
                            codecs: new Codec[]
                            {
                                // Add an AAC Audio layer for the audio encoding
                                new AacAudio(
                                    channels: 2,
                                    samplingRate: 48000,
                                    bitrate: 128000,
                                    profile: AacAudioProfile.AacLc
                                ),
                                // Next, add a H264Video for the video encoding
                               new H264Video (
                                    // Set the GOP interval to 2 seconds for all H264Layers
                                    keyFrameInterval:TimeSpan.FromSeconds(2),
                                     // Add H264Layers. Assign a label that you can use for the output filename
                                    layers:  new H264Layer[]
                                    {
                                        new H264Layer (
                                            bitrate: 3600000, // Units are in bits per second and not kbps or Mbps - 3.6 Mbps or 3,600 kbps
                                            width: "1280",
                                            height: "720",
                                            label: "HD-3600kbps" // This label is used to modify the file name in the output formats
                                        ),
                                        new H264Layer (
                                            bitrate: 1600000, // Units are in bits per second and not kbps or Mbps - 1.6 Mbps or 1600 kbps
                                            width: "960",
                                            height: "540",
                                            label: "SD-1600kbps" // This label is used to modify the file name in the output formats
                                        ),
                                        new H264Layer (
                                            bitrate: 600000, // Units are in bits per second and not kbps or Mbps - 0.6 Mbps or 600 kbps
                                            width: "640",
                                            height: "360",
                                            label: "SD-600kbps" // This label is used to modify the file name in the output formats
                                        ),
                                    }
                                ),
                                // Also generate a set of PNG thumbnails
                                new PngImage(
                                    start: "25%",
                                    step: "25%",
                                    range: "80%",
                                    layers: new PngLayer[]{
                                        new PngLayer(
                                            width: "50%",
                                            height: "50%"
                                        )
                                    }
                                )
                            },
                            // Specify the format for the output files - one for video+audio, and another for the thumbnails
                            formats: new Format[]
                            {
                                // Mux the H.264 video and AAC audio into MP4 files, using basename, label, bitrate and extension macros
                                // Note that since you have multiple H264Layers defined above, you have to use a macro that produces unique names per H264Layer
                                // Either {Label} or {Bitrate} should suffice
                                 
                                new Mp4Format(
                                    filenamePattern:"Video-{Basename}-{Label}-{Bitrate}{Extension}"
                                ),
                                new PngFormat(
                                    filenamePattern:"Thumbnail-{Basename}-{Index}{Extension}"
                                )
                            }
                        ),
                        onError: OnErrorType.StopProcessingJob,
                        relativePriority: Priority.Normal
                    )
                };

                string description = "A simple custom encoding transform with 2 MP4 bitrates";
                // Create the custom Transform with the outputs defined above
                transform = await client.Transforms.CreateOrUpdateAsync(resourceGroupName, accountName, transformName, outputs, description);
            }

            return transform;
        }

        /// <summary>
        /// If the specified transform exists, return that transform. If the it does not
        /// exist, creates a new transform with the specified output. In this case, the
        /// output is set to encode a video using a custom preset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The transform name.</param>
        /// <returns></returns>
        private async Task<Transform> CreateBuiltinTransform(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string transformName)
        {
            // Does a transform already exist with the desired name? Assume that an existing Transform with the desired name
            // also uses the same recipe or Preset for processing content.
            Transform transform = client.Transforms.Get(resourceGroupName, accountName, transformName);

            if (transform == null)
            {
                _logger.LogInformation("Creating a built in h264 mulitibitrate 720 transform...");
                // Create a new Transform Outputs array - this defines the set of outputs for the Transform
                TransformOutput[] outputs = new TransformOutput[]
                {
                  new TransformOutput(
                        new BuiltInStandardEncoderPreset(EncoderNamedPreset.H264MultipleBitrate720p),
                        onError: OnErrorType.StopProcessingJob,
                        relativePriority: Priority.Normal
                    )
                };

                string description = "Azure Standard h264 mulitibitrate 720p ";
                // Create the custom Transform with the outputs defined above
                transform = await client.Transforms.CreateOrUpdateAsync(resourceGroupName, accountName, transformName, outputs, description);
            }

            return transform;
        }

        /// <summary>
        /// Creates a new input Asset and uploads the specified local video file into it.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The asset name.</param>
        /// <param name="fileName">The original file name.</param>
        /// <param name="fileUploadData">The file data want to upload into the asset.</param>
        /// <param name="token">The CancellationToken.</param>
        /// 
        /// <returns></returns>
        private async Task<Asset> CreateInputAssetAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string assetName,
            string fileName,
            byte[] fileUploadData,
            CancellationToken token)
        {
            // In this example, we are assuming that the asset name is unique.
            //
            // If you already have an asset with the desired name, use the Assets.Get method
            // to get the existing asset. In Media Services v3, the Get method on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            Asset asset = await client.Assets.GetAsync(resourceGroupName, accountName, assetName);

            if (asset == null)
            {
                // Call Media Services API to create an Asset.
                // This method creates a container in storage for the Asset.
                // The files (blobs) associated with the asset will be stored in this container.
                _logger.LogInformation("Creating an input asset...");
                asset = await client.Assets.CreateOrUpdateAsync(resourceGroupName, accountName, assetName, new Asset(), token);
            }
            else
            {
                // The asset already exists and we are going to overwrite it. In your application, if you don't want to overwrite
                // an existing asset, use an unique name.
                _logger.LogInformation($"Warning: The asset named {assetName} already exists. It will be overwritten.");
            }

            // Use Media Services API to get back a response that contains
            // SAS URL for the Asset container into which to upload blobs.
            // That is where you would specify read-write permissions 
            // and the expiration time for the SAS URL.
            var response = await client.Assets.ListContainerSasAsync(
                resourceGroupName,
                accountName,
                assetName,
                permissions: AssetContainerPermission.ReadWrite,
                expiryTime: DateTime.UtcNow.AddHours(4).ToUniversalTime(),
                cancellationToken: token);

            var sasUri = new Uri(response.AssetContainerSasUrls.First());

            // Use Storage API to get a reference to the Asset container
            // that was created by calling Asset's CreateOrUpdate method.  
            BlobContainerClient container = new(sasUri);
            BlobClient blob = container.GetBlobClient(fileName);

            // Use Storage API to upload the file into the container in storage.
            _logger.LogInformation("Uploading a media file to the asset...");
            using(var ms = new MemoryStream(fileUploadData))
            {
                await blob.UploadAsync(ms, token);
            }
            return asset;
        }

        /// <summary>
        /// Creates an output asset. The output from the encoding Job must be written to an Asset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The output asset name.</param>
        /// <param name="token">The CancellationToken.</param>
        /// <returns></returns>
        private async Task<Asset> CreateOutputAssetAsync(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string assetName, CancellationToken token)
        {
            // Check if an Asset already exists
            Asset outputAsset = await client.Assets.GetAsync(resourceGroupName, accountName, assetName, token);

            if (outputAsset != null)
            {
                // The asset already exists and we are going to overwrite it. In your application, if you don't want to overwrite
                // an existing asset, use an unique name.
                _logger.LogInformation($"Warning: The asset named {assetName} already exists. It will be overwritten in this sample.");
            }
            else
            {
                _logger.LogInformation("Creating an output asset..");
                outputAsset = new Asset();
            }

            return await client.Assets.CreateOrUpdateAsync(resourceGroupName, accountName, assetName, outputAsset, token);
        }


        /// <summary>
        /// Submits a request to Media Services to apply the specified Transform to a given input video.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The name of the transform.</param>
        /// <param name="jobName">The (unique) name of the job.</param>
        /// <param name="inputAssetName"></param>
        /// <param name="outputAssetName">The (unique) name of the  output asset that will store the result of the encoding job. </param>
        /// <param name="token">The CancellationToken.</param>
        private async Task<Job> SubmitJobAsync(IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string transformName,
            string jobName,
            string inputAssetName,
            string outputAssetName,
            CancellationToken token)
        {
            JobInput jobInput = new JobInputAsset(assetName: inputAssetName);

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(outputAssetName),
            };

            // In this example, we are assuming that the job name is unique.
            //
            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, Get methods on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            Job job;
            try
            {
                _logger.LogInformation("Creating a job...");
                job = await client.Jobs.CreateAsync(
                         resourceGroupName,
                         accountName,
                         transformName,
                         jobName,
                         new Job
                         {
                             Input = jobInput,
                             Outputs = jobOutputs,
                         },
                         token);
            }
            catch (Exception exception)
            {
                if (exception.GetBaseException() is ApiErrorException apiException)
                {
                    _logger.LogError(apiException, 
                        $"ERROR: API call failed with error code '{apiException.Body.Error.Code}' and message '{apiException.Body.Error.Message}'.");
                }
                throw exception;
            }
            return job;
        }



        /// <summary>
        /// Polls Media Services for the status of the Job.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The name of the transform.</param>
        /// <param name="jobName">The name of the job you submitted.</param>
        /// <param name="token">The CancellationToken.</param>
        /// <returns></returns>
        private async Task<Job> WaitForJobToFinishAsync(IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string transformName,
            string jobName,
            Action<string> progess,
            CancellationToken token)
        {
            const int SleepIntervalMs = 30 * 1000;

            Job job;

            do
            {
                job = await client.Jobs.GetAsync(resourceGroupName, accountName, transformName, jobName, token);

                _logger.LogInformation($"Job is '{job.State}'.");
                progess($"Job is '{job.State}'.");
                for (int i = 0; i < job.Outputs.Count; i++)
                {
                    JobOutput output = job.Outputs[i];
                    _logger.LogInformation($"\tJobOutput[{i}] is '{output.State}'.");
                    progess($"JobOutput[{i}] is '{output.State}'.");

                    if (output.State == JobState.Processing)
                    {
                        _logger.LogInformation($"  Progress: '{output.Progress}'.");
                        progess($"{output.Label}: '{output.Progress}'.");
                    }
                }

                if (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled)
                {
                    await Task.Delay(SleepIntervalMs);
                }
            }
            while (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled);

            return job;
        }

        /// <summary>
        /// Downloads the specified output asset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The output asset.</param>
        /// <param name="outputFolderName">The name of the folder into which to download the results.</param>
        /// <param name="progess">Action for call back.</param>
        /// <param name="downloadAssets">Boolean whether to persist assets to disk.</param>
        /// <param name="token">The CancellationToken.</param>
        /// <returns></returns>
        private async Task<IList<string>> GenerateResults(
            IAzureMediaServicesClient client, 
            string resourceGroupName, 
            string accountName,
            string assetName, 
            string outputFolderName, 
            Action<string> progess, 
            bool downloadAssets,
            CancellationToken token)
        {
            var list = new List<string>();
            // Use Media Service and Storage APIs to download the output files to a local folder
            AssetContainerSas assetContainerSas = client.Assets.ListContainerSas(
                            resourceGroupName,
                            accountName,
                            assetName,
                            permissions: AssetContainerPermission.Read,
                            expiryTime: DateTime.UtcNow.AddHours(1).ToUniversalTime()
                            );

            Uri containerSasUrl = new(assetContainerSas.AssetContainerSasUrls.FirstOrDefault());
            BlobContainerClient container = new(containerSasUrl);

            var downloadFolder = Path.Combine(_environment.WebRootPath, outputFolderName);

            string directory = Path.Combine(downloadFolder, assetName);
            Directory.CreateDirectory(directory);
            // string urlPrefix = directory.Replace(_environment.WebRootPath, string.Empty).Replace(@"\", "/"); 
            _logger.LogInformation($"Downloading results to {directory}.");
            progess("Downloading results..");

            string continuationToken = null;

            // Call the listing operation and enumerate the result segment.
            // When the continuation token is empty, the last segment has been returned
            // and execution can exit the loop.
            do
            {
                var resultSegment = container.GetBlobs().AsPages(continuationToken);

                foreach (Azure.Page<Azure.Storage.Blobs.Models.BlobItem> blobPage in resultSegment)
                {
                    foreach (Azure.Storage.Blobs.Models.BlobItem blobItem in blobPage.Values)
                    {

                        var blobClient = container.GetBlobClient(blobItem.Name);
                        string filename = Path.Combine(directory, blobItem.Name);
                        // var url = Path.Combine(urlPrefix, blobItem.Name);
                        progess($"Retriving file {blobClient.Uri.AbsoluteUri}");
                        if(downloadAssets) 
                        {
                            await blobClient.DownloadToAsync(filename);
                        }
                        if(Path.GetExtension(filename) == ".png" || Path.GetExtension(filename) == ".jpg")
                        {
                            list.Add(blobClient.Uri.AbsoluteUri);
                        }
                    }

                    // Get the continuation token and loop until it is empty.
                    continuationToken = blobPage.ContinuationToken;
                }

            } while (continuationToken != "");

            _logger.LogInformation("Download complete.");
            return list;
        }

        /// <summary>
        /// Creates a StreamingLocator for the specified asset and with the specified streaming policy name.
        /// Once the StreamingLocator is created the output asset is available to clients for playback.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The name of the output asset.</param>
        /// <param name="locatorName">The StreamingLocator name (unique in this case).</param>
        /// <param name="token">The CancellationToken.</param>
        /// <returns></returns>
        private async Task<StreamingLocator> CreateStreamingLocatorAsync(
            IAzureMediaServicesClient client,
            string resourceGroup,
            string accountName,
            string assetName,
            string locatorName,
            CancellationToken token)
        {
            StreamingLocator locator = await client.StreamingLocators.CreateAsync(
                resourceGroup,
                accountName,
                locatorName,
                new StreamingLocator
                {
                    AssetName = assetName,
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly
                },
                token);

            return locator;
        }

        /// <summary>
        /// Checks if the streaming endpoint is in the running state,
        /// if not, starts it.
        /// Then, builds the streaming URLs.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="locatorName">The name of the StreamingLocator that was created.</param>
        /// <param name="streamingEndpoint">The streaming endpoint.</param>
        /// <param name="token">The CancellationToken.</param>
        /// <returns></returns>
        private async Task<IList<string>> GetStreamingUrlsAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            String locatorName,
            StreamingEndpoint streamingEndpoint,
            CancellationToken token)
        {
            IList<string> streamingUrls = new List<string>();

            ListPathsResponse paths = await client.StreamingLocators.ListPathsAsync(resourceGroupName, accountName, locatorName, token);

            foreach (StreamingPath path in paths.StreamingPaths)
            {
                _logger.LogInformation($"The following formats are available for {path.StreamingProtocol.ToString().ToUpper()}:");
                foreach (string streamingFormatPath in path.Paths)
                {
                    UriBuilder uriBuilder = new()
                    {
                        Scheme = "https",
                        Host = streamingEndpoint.HostName,

                        Path = streamingFormatPath
                    };
                    _logger.LogInformation($"\t{uriBuilder}");
                    streamingUrls.Add(uriBuilder.ToString());
                }
            }

            return streamingUrls;
        }


        /// <summary>
        /// Delete the job and assets and streaming locator that were created.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The transform name.</param>
        /// <param name="jobName">The job name.</param>
        /// <param name="inputAssetName">The input asset name.</param>
        /// <param name="outputAssetName">The output asset name.</param>
        /// <param name="streamingLocatorName">The streaming locator name. </param>
        /// <param name="stopEndpoint">Stop endpoint if true, keep endpoint running if false.</param>
        /// <param name="streamingEndpointName">The endpoint name.</param>
        /// <returns>A task.</returns>
        private async Task CleanUpAsync(IAzureMediaServicesClient client, string resourceGroupName, string accountName,
            string transformName, string jobName, string inputAssetName, string outputAssetName, string streamingLocatorName,
            bool stopEndpoint, string streamingEndpointName)
        {
            await client.Jobs.DeleteAsync(resourceGroupName, accountName, transformName, jobName);
            await client.Assets.DeleteAsync(resourceGroupName, accountName, inputAssetName);
            await client.Assets.DeleteAsync(resourceGroupName, accountName, outputAssetName);
            await client.StreamingLocators.DeleteAsync(resourceGroupName, accountName, streamingLocatorName);

            if (stopEndpoint)
            {
                // Because we started the endpoint, we'll stop it.
                await client.StreamingEndpoints.StopAsync(resourceGroupName, accountName, streamingEndpointName);
            }
            else
            {
                // We will keep the endpoint running because it was not started by us. There are costs to keep it running.
                // Please refer https://azure.microsoft.com/en-us/pricing/details/media-services/ for pricing. 
                _logger.LogInformation($"The endpoint '{streamingEndpointName}' is running. To halt further billing on the endpoint, please stop it in azure portal or AMS Explorer.");
            }
        }
    }
}

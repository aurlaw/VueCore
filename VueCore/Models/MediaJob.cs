namespace VueCore.Models
{
    public class MediaJob
    {
        public MediaJob(string jobName, string locatorName, string inputAssetName, string outputAssetName, bool stopEndpoint, string streamUrl)
        {
            JobName = jobName;
            LocatorName = locatorName;
            InputAssetName = inputAssetName;
            OutputAssetName = outputAssetName;
            StopEndpoint = stopEndpoint;
            StreamUrl = streamUrl;
        }

        public string JobName {get;set;}
        public string LocatorName  {get;set;}
        public string  InputAssetName  {get;set;}
        public string  OutputAssetName  {get;set;}
        public bool StopEndpoint  {get;set;}   
        public string StreamUrl {get;set;}

        
    }
}

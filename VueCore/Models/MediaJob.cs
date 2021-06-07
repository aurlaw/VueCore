using System.Collections.Generic;

namespace VueCore.Models
{
    public class MediaJob
    {
        public MediaJob(string jobName, string locatorName, string inputAssetName, string outputAssetName, bool stopEndpoint, IList<string> streamUrlList, string thumbnail, string title)
        {
            JobName = jobName;
            LocatorName = locatorName;
            InputAssetName = inputAssetName;
            OutputAssetName = outputAssetName;
            StopEndpoint = stopEndpoint;
            StreamUrlList = streamUrlList;
            Thumbnail = thumbnail;
            Title = title;
        }
        public string Title {get;set;}
        public string JobName {get;set;}
        public string LocatorName  {get;set;}
        public string  InputAssetName  {get;set;}
        public string  OutputAssetName  {get;set;}
        public bool StopEndpoint  {get;set;}   
        public string Thumbnail {get;set;}
        public IList<string> StreamUrlList {get;set;}

        
    }
}

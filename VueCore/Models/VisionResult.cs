namespace VueCore.Models
{
    public class VisionResult
    {
        public bool Success {get;set;}
        public string Url {get;set;}
        public VisionAnalysis Analysis {get;set;}
        public string ThumbnailUrl {get;set;}
    }
}


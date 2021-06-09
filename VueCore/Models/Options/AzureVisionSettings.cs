namespace VueCore.Models.Options
{
    public class AzureVisionSettings
    {
        public string Endpoint {get;set;}
        public string SubscriptionKey {get;set;}

        public int ThumbnailWidth {get;set;}
        public int ThumbnailHeight {get;set;}
    }
}
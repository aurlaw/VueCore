using System.Threading.Tasks;

namespace VueCore.Hubs
{
    public interface IMediaProcessHubClient
    {
         Task SendUploaded(string fileName);
         Task SendReceived(string fileName);
         Task SendProcessed(string mediaUrl);
         Task SetGroupId(string groupId);
         Task SendError(string error);
    }
}

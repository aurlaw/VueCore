using System.Threading.Tasks;

namespace VueCore.Hubs
{
    public interface IMediaProcessHubClient
    {
         Task SendReceived(string fileName);
         Task SendProcessed(string mediaUrl);
         Task SetGroupId(string groupId);
    }
}

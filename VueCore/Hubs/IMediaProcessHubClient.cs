using System.Threading.Tasks;
using VueCore.Models;

namespace VueCore.Hubs
{
    public interface IMediaProcessHubClient
    {
         Task SendUploaded(string fileName);
         Task SendReceived(MediaJob mediaJob);
         Task SendProgress(string message);
         Task SetGroupId(string groupId);
         Task SendError(string error);
    }
}

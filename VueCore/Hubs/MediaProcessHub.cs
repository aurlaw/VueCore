using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace VueCore.Hubs
{
    public class MediaProcessHub : Hub<IMediaProcessHubClient>
    {
        private readonly ILogger<MediaProcessHub> _logger;

        public MediaProcessHub(ILogger<MediaProcessHub> logger)
        {
            _logger = logger;
        }

        public async Task GenerateGroupId()
        {
            var groupId = $"grp-{Guid.NewGuid()}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SetGroupId(groupId);
        }

    }
}
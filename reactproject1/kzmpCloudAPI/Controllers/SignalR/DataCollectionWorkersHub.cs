using Microsoft.AspNetCore.SignalR;

namespace kzmpCloudAPI.Controllers.SignalR
{
    public class DataCollectionWorkersHub : Hub
    {
        public async Task Send(string message)
        {
            await Clients.All.SendAsync(message);
        }
    }
}

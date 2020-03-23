using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GuardRail.Api
{
    public sealed class GuardRailHub : Hub
    {
        public async Task Send(string user, string message) =>
            await Clients.All.SendCoreAsync(
                "ReceiveMessage",
                new object[]
                {
                    user,
                    message
                });
    }
}
using System;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using Microsoft.AspNetCore.SignalR;

namespace GuardRail.Api
{
    public sealed class GuardRailHub : Hub
    {
        public async Task SendLogAsync(
            Log message)
        {
            if (Clients?.All != null)
            {
                await Clients.All.SendCoreAsync(
                    "NewLog",
                    new object[]
                    {
                        message
                    });
            }
        }

        public async Task SendAsync(
            Guid userId,
            string methodName,
            string message) =>
            await Clients
                .User(userId.ToString())
                .SendCoreAsync(
                    methodName,
                    new object[]
                    {
                        message
                    });
    }
}
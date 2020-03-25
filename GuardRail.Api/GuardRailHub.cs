using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GuardRail.Api
{
    public sealed class GuardRailHub : Hub
    {
        public async Task SendLogAsync(
            string message) =>
            await Clients.All.SendCoreAsync(
                "NewLog",
                new object[]
                {
                    message
                });

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
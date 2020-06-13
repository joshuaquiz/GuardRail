using System;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Api;
using GuardRail.Api.AccessControlDevices.ACR1252U;
using GuardRail.Api.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using PCSC;
using Serilog;
using Xunit;

namespace GuardRail.Tests.AccessControlDevices.ACR1252U
{
    public static class Acr1252UFactoryTests
    {
        [Fact]
        public static async Task TestGetAccessControlDevices()
        {
            using (var guardRailContext = new GuardRailContext(
                new DbContextOptionsBuilder<GuardRailContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options))
            {
                var readers = new[]
                {
                    "ACS ACR1252 Dual Reader PICC 0",
                    "ACS ACR1252 Dual Reader SAM 0"
                };
                await guardRailContext.AccessControlDevices.AddRangeAsync(
                    readers.Select(x =>
                        new AccessControlDevice
                        {
                            DeviceId = x
                        }));
                var mockSCardContext = new Mock<ISCardContext>(MockBehavior.Strict);
                mockSCardContext
                    .Setup(x => x.Establish(SCardScope.System));
                mockSCardContext
                    .Setup(x => x.Release());
                var mockGuardRailLogger = new Mock<IGuardRailLogger>(MockBehavior.Strict);
                mockGuardRailLogger
                    .Setup(x => x.LogAsync("2 access control devices were found"))
                    .Returns(Task.CompletedTask);
                mockSCardContext
                    .Setup(x => x.GetReaders())
                    .Returns(readers)
                    .Verifiable();
                var acr1252UFactory = new Acr1252UFactory(
                    null,
                    mockSCardContext.Object,
                    null,
                    guardRailContext,
                    mockGuardRailLogger.Object);
                var devices = await acr1252UFactory.GetAccessControlDevices();
                mockSCardContext.VerifyAll();
                mockGuardRailLogger.VerifyAll();
                Assert.Equal(readers.Length, devices.Count);
            }
        }
    }
}
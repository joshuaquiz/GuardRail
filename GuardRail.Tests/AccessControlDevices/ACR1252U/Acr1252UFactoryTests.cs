using System;
using System.Threading.Tasks;
using GuardRail.AccessControlDevices.ACR1252U;
using Moq;
using PCSC;
using Xunit;

namespace GuardRail.Tests.AccessControlDevices.ACR1252U
{
    public static class Acr1252UFactoryTests
    {
        [Fact]
        public static async Task TestGetAccessControlDevices()
        {
            var mockSCardContext = new Mock<ISCardContext>();
            mockSCardContext
                .Setup(x => x.Establish(SCardScope.System))
                .Verifiable();
            var readers = new[]
            {
                Guid.NewGuid().ToString()
            };
            mockSCardContext
                .Setup(x => x.GetReaders())
                .Returns(readers)
                .Verifiable();
            var acr1252UFactory = new Acr1252UFactory(
                null,
                mockSCardContext.Object);
            var devices = await acr1252UFactory.GetAccessControlDevices();
            mockSCardContext.Verify();
            Assert.Equal(1, devices.Count);
        }
    }
}
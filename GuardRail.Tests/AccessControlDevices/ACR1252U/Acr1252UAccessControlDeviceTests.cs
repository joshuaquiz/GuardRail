using System;
using GuardRail.AccessControlDevices.ACR1252U;
using Xunit;

namespace GuardRail.Tests.AccessControlDevices.ACR1252U
{
    public static class Acr1252UAccessControlDeviceTests
    {
        [Fact]
        public static void TestSCardContextNotNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    Acr1252UAccessControlDevice.Create(
                        null,
                        null,
                        null));
        }
    }
}
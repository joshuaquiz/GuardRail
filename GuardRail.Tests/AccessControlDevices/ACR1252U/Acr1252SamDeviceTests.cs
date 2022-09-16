using System;
using GuardRail.Api.AccessControlDevices.ACR1252U;
using Xunit;

namespace GuardRail.Tests.AccessControlDevices.ACR1252U;

public static class Acr1252SamDeviceTests
{
    [Fact]
    public static void TestSCardContextNotNull() =>
        Assert.Throws<ArgumentNullException>(
            () =>
                Acr1252PiccDevice.Create(
                    null,
                    null,
                    null,
                    null,
                    null));
}
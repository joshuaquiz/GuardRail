using System;
using System.Net;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public sealed record UdpResponse(
    Guid RequestId,
    string CommandName,
    string? Body,
    IPEndPoint ReceivedFrom);
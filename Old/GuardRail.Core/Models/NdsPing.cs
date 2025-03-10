using System;

namespace GuardRail.Core.Models;

/// <summary>
/// A ping request/response.
/// </summary>
/// <remarks>
/// A client will send this data and the server will send it back to show it received it.
/// </remarks>
public sealed class NdsPing
{
    /// <summary>
    /// The ID of the request.
    /// </summary>
    public Guid RequestId { get; set; }
}
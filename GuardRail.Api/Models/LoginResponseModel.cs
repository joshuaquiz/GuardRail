using System;

namespace GuardRail.Api.Models;

public sealed record LoginResponseModel(
    bool Succeeded,
    string? Email,
    string? Name,
    Guid? Token,
    bool NeedsPasswordReset);
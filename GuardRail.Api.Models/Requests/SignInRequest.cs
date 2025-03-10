namespace GuardRail.Api.Models.Requests;

/// <summary>
/// A sign in API request.
/// </summary>
/// <param name="Email">The users email.</param>
/// <param name="Password">The users password.</param>
public sealed record SignInRequest(
    string Email,
    string Password);
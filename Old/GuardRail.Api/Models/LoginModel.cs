namespace GuardRail.Api.Models;

public sealed record LoginModel(
    string Username,
    string Password);
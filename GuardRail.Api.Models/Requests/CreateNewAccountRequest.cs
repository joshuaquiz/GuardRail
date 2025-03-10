namespace GuardRail.Api.Models.Requests;

/// <summary>
/// An API request with the data needed to create a new account.
/// </summary>
/// <param name="AccountName">The name of the new account.</param>
/// <param name="FirstName">The first name of the new user.</param>
/// <param name="LastName">The last name of the new user.</param>
/// <param name="Phone">The phone of the new user.</param>
/// <param name="Email">The email of the new user.</param>
public sealed record CreateNewAccountRequest(
    string AccountName,
    string FirstName,
    string LastName,
    string Phone,
    string Email);
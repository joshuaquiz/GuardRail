using System;
using GuardRail.Core.Models.Enums;

namespace GuardRail.Api.Models.Requests;

/// <summary>
/// An API request with the data needed to create a new user.
/// </summary>
/// <param name="AccountId">The ID of the account.</param>
/// <param name="FirstName">The first name of the new user.</param>
/// <param name="LastName">The last name of the new user.</param>
/// <param name="Phone">The user's phone.</param>
/// <param name="Email">The user's email.</param>
/// <param name="UserType">The user's type.</param>
public sealed record CreateNewUserRequest(
    Guid AccountId,
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    UserType UserType);
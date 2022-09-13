using System;

namespace GuardRail.Api.Controllers.Models;

public sealed class UserModel
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}
namespace GuardRail.Core.Enums;

/// <summary>
/// Types of users.
/// </summary>
public enum UserType : byte
{
    Normal = 0,

    SuperAdmin = 1,

    AccountAdmin = 2,

    LocationAdmin = 3
}
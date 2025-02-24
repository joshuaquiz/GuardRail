namespace GuardRail.Core.Exceptions;

/// <summary>
/// An exceptions that indicates an invalid email and password was used.
/// </summary>
public sealed class InvalidEmailPasswordException : UserNotAllowedToSignInException
{
    /// <summary>
    /// The email used in the signin attempt.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// An exceptions that indicates an invalid email and password was used.
    /// </summary>
    /// <param name="email">The email used in the signin attempt.</param>
    public InvalidEmailPasswordException(
        string email)
        : base(
            $"The provided email '{email}' and provided password '***' returned no records.")
    {
        ExternalMessage = "The provided email and password were invalid";
        Email = email;
    }
}
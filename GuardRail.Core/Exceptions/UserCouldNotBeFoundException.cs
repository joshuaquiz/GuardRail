namespace GuardRail.Core.Exceptions;

/// <summary>
/// The user was unable to be located.
/// </summary>
public sealed class UserCouldNotBeFoundException : GuardRailExceptionBase
{
    public UserCouldNotBeFoundException(
        string reason)
        : base(
            reason)
    {
        ExternalMessage = "Invalid record";
    }
}
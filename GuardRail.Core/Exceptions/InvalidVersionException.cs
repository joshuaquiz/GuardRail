namespace GuardRail.Core.Exceptions;

/// <summary>
/// The provided version was invalid.
/// </summary>
public sealed class InvalidVersionException : GuardRailExceptionBase
{
    public InvalidVersionException(
        string reason)
        : base(
            reason)
    {
        ExternalMessage = "The provided version is invalid";
    }
}
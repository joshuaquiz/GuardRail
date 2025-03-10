using System;

namespace GuardRail.Core.Exceptions;

/// <summary>
/// Base class for all exceptions that indicate that a user is not allowed to sign in.
/// </summary>
public abstract class UserNotAllowedToSignInException : GuardRailExceptionBase
{
    protected UserNotAllowedToSignInException(
        string message)
        : base(
            message)
    {
    }

    protected UserNotAllowedToSignInException(
        string message,
        Exception innerException)
        : base(
            message,
            innerException)
    {
    }
}
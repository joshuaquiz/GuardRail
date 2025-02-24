using System;

namespace GuardRail.Core.Exceptions;

/// <summary>
/// A base class for all exceptions.
/// </summary>
public abstract class GuardRailExceptionBase : Exception
{
    private string? _externalMessage;

    /// <summary>
    /// A message field that can be used to provide a friendly-er message externally.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Exception.Message"/> if no value is set.
    /// </remarks>
    public string ExternalMessage
    {
        get => _externalMessage ?? Message;
        protected set => _externalMessage = value;
    }

    protected GuardRailExceptionBase(
        string message)
        : base(
            message)
    {
    }

    protected GuardRailExceptionBase(
        string message,
        Exception innerException)
        : base(
            message,
            innerException)
    {
    }
}
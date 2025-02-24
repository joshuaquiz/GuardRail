using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Exceptions;
using GuardRail.Logic.Models;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// An interface defining high-level actions for version management.
/// </summary>
public interface IVersionManagementService
{
    /// <summary>
    /// Checks to see if the provided version is up to date.
    /// </summary>
    /// <param name="version">The version to check.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="VersionCheckResult"/> representing the work to do the version check.</returns>
    /// <exception cref="InvalidVersionException">Thrown when <paramref name="version"/> is greater than the newest known version.</exception>
    public Task<VersionCheckResult> VersionCheck(
        Version version,
        CancellationToken cancellationToken);
}
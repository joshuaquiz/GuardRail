using System.Collections.Generic;

namespace GuardRail.Core;

/// <summary>
/// The configuration for an update/install.
/// </summary>
/// <param name="LatestVersion">The latest version of the software.</param>
/// <param name="InstallFiles">The files for the upload.</param>
/// <param name="RestartCommand">The command to run that will do any last cleanups and start the updated application.</param>
/// <param name="UpdateDirectory">The directory that the updates are to be stored in.</param>
public sealed record InstallConfiguration(
    string LatestVersion,
    IReadOnlyCollection<InstallFile> InstallFiles,
    string RestartCommand,
    string UpdateDirectory);
using System.Collections.Generic;

namespace GuardRail.Core;

/// <summary>
/// The configuration for an update/install.
/// </summary>
public sealed class InstallConfiguration
{
    /// <summary>
    /// The latest version of the software.
    /// </summary>
    public string LatestVersion { get; set; }

    /// <summary>
    /// The files for the upload.
    /// </summary>
    public IReadOnlyCollection<InstallFile> InstallFiles { get; set; }

    /// <summary>
    /// The command to run that will do any last cleanups and start the updated application.
    /// </summary>
    public string RestartCommand { get; set; }

    /// <summary>
    /// The directory that the updates are to be stored in.
    /// </summary>
    public string UpdateDirectory { get; set; }
}
using System;

namespace GuardRail.Core;

/// <summary>
/// Represents a file to be downloaded for the install.
/// </summary>
/// <param name="DownloadUri">The Location of the file on the web.</param>
/// <param name="LocalPath">Where the file goes locally.</param>
public sealed record InstallFile(
    Uri DownloadUri,
    string LocalPath);
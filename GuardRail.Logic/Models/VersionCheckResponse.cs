using System;

namespace GuardRail.Logic.Models;

/// <summary>
/// The result of a version check.
/// </summary>
/// <param name="IsLatest">Whether the inquired-about version is the latest.</param>
/// <param name="IsUpdateRequired">Whether the update is a required update, only provided if not the latest.</param>
/// <param name="LatestVersion">The latest version number, only provided if not the latest.</param>
/// <param name="DownloadUrl">The url do download the new version, only provided if not the latest.</param>
public sealed record VersionCheckResult(
    bool IsLatest,
    bool? IsUpdateRequired,
    Version? LatestVersion,
    Uri? DownloadUrl);
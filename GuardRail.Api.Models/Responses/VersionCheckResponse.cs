using System;
using System.Collections.Generic;

namespace GuardRail.Api.Models.Responses;

/// <summary>
/// The response to a version check.
/// </summary>
/// <param name="IsLatest">Whether the inquired-about version is the latest.</param>
/// <param name="IsUpdateRequired">Whether the update is a required update.</param>
/// <param name="LatestVersion">The latest version number, only provided if not the latest.</param>
/// <param name="InstallFiles">The files for the upload, only provided if not the latest.</param>
public sealed record VersionCheckResponse(
    bool IsLatest,
    bool? IsUpdateRequired,
    Version? LatestVersion,
    IReadOnlyCollection<KeyValuePair<string, string>>? InstallFiles);
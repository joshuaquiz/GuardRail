using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// An item representing a local app version.
/// </summary>
public class VersionHistory
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The version number.
    /// </summary>
    public required Version Version { get; set; }

    /// <summary>
    /// Whether this version is required.
    /// </summary>
    public required bool IsRequired { get; set; }
}
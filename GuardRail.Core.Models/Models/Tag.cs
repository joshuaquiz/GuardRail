using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// Represents a tag.
/// </summary>
public class Tag
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the account.
    /// </summary>
    public required Guid AccountGuid { get; set; }

    /// <summary>
    /// The Name of the tag.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The ID of the item this tag is for.
    /// </summary>
    public required Guid TaggedItemGuid { get; set; }

    /// <summary>
    /// the name of the item this tag is for.
    /// </summary>
    public required string TaggedItemName { get; set; }
}
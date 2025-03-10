using System;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A camera on a map.
/// </summary>
public class MapCamera : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The X of the camera.
    /// </summary>
    [Required]
    public long X { get; set; }

    /// <summary>
    /// The Y of the camera.
    /// </summary>
    [Required]
    public long Y { get; set; }

    /// <summary>
    /// The rotation degrees of the camera.
    /// </summary>
    [Required]
    public decimal Rotation { get; set; }

    /// <summary>
    /// The view field length of the camera.
    /// </summary>
    [Required]
    public int ViewFieldLength { get; set; }

    /// <summary>
    /// The view field degrees of the camera.
    /// </summary>
    [Required]
    public decimal ViewFieldDegrees { get; set; }
}
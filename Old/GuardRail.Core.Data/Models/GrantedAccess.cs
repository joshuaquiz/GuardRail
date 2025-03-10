using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// An access reference table.
/// </summary>
public class GrantedAccess : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// Whether or not the access defined is a allow or deny.
    /// </summary>
    [Required]
    public bool IsDenied { get; set; } = false;

    /// <summary>
    /// The Guid of the <see cref="Models.User"/>, if there is one.
    /// </summary>
    public Guid? UserGuid { get; set; }

    /// <summary>
    /// The <see cref="Models.User"/>, if there is one.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// The Guid of the <see cref="UserGroup"/>, if there is one.
    /// </summary>
    public Guid? UserGroupGuid { get; set; }

    /// <summary>
    /// The <see cref="UserGroup"/>, if there is one.
    /// </summary>
    public UserGroup? UserGroup { get; set; }

    /// <summary>
    /// The <see cref="AccessPointGroup"/>s, if there are any.
    /// </summary>
    public List<AccessPointGroup>? AccessPointGroups { get; set; }

    /// <summary>
    /// The <see cref="AccessPoint"/>s, if there are any.
    /// </summary>
    public List<AccessPoint>? AccessPoints { get; set; }

    /// <summary>
    /// A list of <see cref="Restriction"/>s, if there are any.
    /// </summary>
    public List<Restriction>? Restrictions { get; set; }
}
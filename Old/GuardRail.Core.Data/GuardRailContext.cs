using GuardRail.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Core.Data;

/// <summary>
/// Database context for the project.
/// </summary>
public class GuardRailContext : DbContext
{
    /// <summary>
    /// Creates a new <see cref="GuardRailContext"/>.
    /// </summary>
    /// <param name="options"></param>
    public GuardRailContext(DbContextOptions options)
        : base(options)
    {
    }

    /// <summary>
    /// The <see cref="Account"/>s in the database.
    /// </summary>
    public DbSet<Account> Accounts { get; set; }

    /// <summary>
    /// The <see cref="Log"/>s in the database.
    /// </summary>
    public DbSet<Log> Logs { get; set; }

    /// <summary>
    /// The <see cref="UnlockAccessToken"/>s in the database.
    /// </summary>
    public DbSet<UnlockAccessToken> UnlockAccessTokens { get; set; }

    /// <summary>
    /// The <see cref="User"/>s in the database.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// The <see cref="ApiAccessKey"/>s in the database.
    /// </summary>
    public DbSet<ApiAccessKey> ApiAccessKeys { get; set; }

    /// <summary>
    /// The <see cref="UserGroup"/>s in the database.
    /// </summary>
    public DbSet<UserGroup> UserGroups { get; set; }

    /// <summary>
    /// The <see cref="UserAccessMethod"/>s in the database.
    /// </summary>
    public DbSet<UserAccessMethod> UserAccessMethods { get; set; }

    /// <summary>
    /// The <see cref="AccessPoint"/>s in the database.
    /// </summary>
    public DbSet<AccessPoint> AccessPoints { get; set; }

    /// <summary>
    /// The <see cref="AccessPointGroup"/>s in the database.
    /// </summary>
    public DbSet<AccessPointGroup> AccessPointGroups { get; set; }

    /// <summary>
    /// The <see cref="Door"/>s in the database.
    /// </summary>
    public DbSet<Door> Doors { get; set; }

    /// <summary>
    /// The <see cref="GrantedAccess"/>s in the database.
    /// </summary>
    public DbSet<GrantedAccess> GrantedAccesses { get; set; }

    /// <summary>
    /// The <see cref="Restriction"/>s in the database.
    /// </summary>
    public DbSet<Restriction> Restrictions { get; set; }

    /// <summary>
    /// The <see cref="Map"/>s in the database.
    /// </summary>
    public DbSet<Map> Maps { get; set; }

    /// <summary>
    /// The <see cref="Floor"/>s in the database.
    /// </summary>
    public DbSet<Floor> Floors { get; set; }

    /// <summary>
    /// The <see cref="Room"/>s in the database.
    /// </summary>
    public DbSet<Room> Rooms { get; set; }

    /// <summary>
    /// The <see cref="MapDoor"/>s in the database.
    /// </summary>
    public DbSet<MapDoor> MapDoors { get; set; }

    /// <summary>
    /// The <see cref="MapCamera"/>s in the database.
    /// </summary>
    public DbSet<MapCamera> MapCameras { get; set; }
}
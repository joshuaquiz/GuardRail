using System;
using System.Threading.Tasks;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data;

public sealed class AccessControlDevice : IAccessControlDevice
{
    public Guid Id { get; set; }

    public string DeviceId { get; set; }

    public string FriendlyName { get; set; }

    public bool IsConfigured { get; set; }

    public Door Door { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<AccessControlDevice>()
            ?.HasKey(b => b.Id);
        builder
            ?.Entity<AccessControlDevice>()
            ?.HasIndex(b => b.DeviceId);
        builder
            ?.Entity<AccessControlDevice>()
            ?.HasMany<Door>();
    }

    public void Dispose()
    {

    }

    public Task Init() =>
        Task.CompletedTask;

    public Task<string> GetDeviceId() =>
        Task.FromResult(DeviceId);

    public Task PresentNoAccessGranted(string reason) =>
        Task.CompletedTask;
}
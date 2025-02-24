using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Local.Service;

public static class Program
{
    public static async Task Main(
        string[] args)
    {
        var builder = WebApplication.CreateBuilder(
            args);
        builder.Services.AddWindowsService();
        builder.Services.AddHostedService<AutoUpdateCheckerWorker>();
        builder.Services.AddHostedService<DataSyncWorker>();
        builder.Services.AddHostedService<UdpPingListenerWorker>();
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}
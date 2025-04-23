using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public sealed class Program
{
    public static void Main(string[] args) =>
        CreateHostBuilder(args)
            .Build()
            .Run();

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                x =>
                    x.UseStartup<Startup>());
}
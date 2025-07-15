using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public static class Program
{
    public static async Task Main(
        string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                x =>
                    x.UseStartup<Startup>()
                        .UseUrls("https://localhost:5001/"));

    private static void UnhandledExceptionTrapper(
        object sender,
        UnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e.ExceptionObject.ToString());
        Environment.Exit(1);
    }
}
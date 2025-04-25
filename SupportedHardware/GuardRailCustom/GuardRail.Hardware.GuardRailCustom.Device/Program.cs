using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public sealed class Program
{
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        CreateHostBuilder(args)
            .Build()
            .Run();
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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

namespace GuardRail.Api;

public static class ApiSetupExtensions
{
    public static void AddGuardRailApi(
        this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi(
            "GuardRail",
            x =>
            {
                x.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
            });
    }

    public static void UseGuardRailApi(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}
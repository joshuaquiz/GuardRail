using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces;
using Microsoft.AspNetCore.Builder;

namespace GuardRail.DeviceLogic.DependencyHelpers;

/// <summary>
/// Extra DI helpers.
/// </summary>
public static class DependencyInjectionHelpers
{
    public static IApplicationBuilder UseAsyncInitTypes(this IApplicationBuilder applicationBuilder)
    {
        var inits = applicationBuilder.ApplicationServices.GetService<IEnumerable<IAsyncInit>>()
                    ?? Array.Empty<IAsyncInit>();
        Task.WhenAll(inits.Select(async x => await x.InitAsync())).GetAwaiter().GetResult();
        return applicationBuilder;
    }
}
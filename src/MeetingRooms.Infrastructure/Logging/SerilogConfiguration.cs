using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace MeetingRooms.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static IHostBuilder UseConfiguredSerilog(this IHostBuilder host) =>
        host.UseSerilog((ctx, cfg) =>
        {
            cfg.MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .WriteTo.Console();
        });
}

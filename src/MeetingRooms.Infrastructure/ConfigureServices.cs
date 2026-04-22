using MeetingRooms.Application.Abstractions;
using MeetingRooms.Infrastructure.UserContext;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingRooms.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, HttpUserContext>();
        return services;
    }
}

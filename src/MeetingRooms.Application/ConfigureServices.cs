using FluentValidation;
using MeetingRooms.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingRooms.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ConfigureServices).Assembly));

        services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}

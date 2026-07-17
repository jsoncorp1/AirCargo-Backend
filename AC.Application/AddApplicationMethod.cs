using System.Reflection;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Abstractions.Messaging.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace AC.Application;

public static class AddApplicationMethod
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        Assembly assembly = typeof(AddApplicationMethod).Assembly;

        RegisterHandlers(services, assembly, typeof(ICommandHandler<>));
        RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));
        RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));

        return services;
    }

    private static void RegisterHandlers(
        IServiceCollection services, Assembly assembly, Type openInterface)
    {
        var registrations = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == openInterface)
                .Select(i => new { Service = i, Implementation = t }));

        foreach (var reg in registrations)
            services.AddScoped(reg.Service, reg.Implementation);
    }
}
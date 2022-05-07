using System.Reflection;
using EasySourcing.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.DependencyInjection;

public static class EventSourcingDependencyInjection
{
    public static IEventSourcingBuilder AddEventSourcing(this IServiceCollection services,
        Action<EventSourcingOptions> setupAction = null)
    {
        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        services.AddScoped(typeof(IEventSourcedRepository<>), typeof(EventSourcedRepository<>));

        return new EventSourcingOptionsBuilder(services);
    }

    public static IEventSourcingBuilder AddProjection(this IEventSourcingBuilder builder, params Assembly[] assemblies)
    {
        if (assemblies == null || !assemblies.Any())
        {
            throw new ArgumentException(
                "No assemblies found to scan. Supply at least one assembly to scan for handlers.");
        }

        builder.Services.AddScoped<IProjector, Projector>();

        foreach (var implementationType in assemblies.SelectMany(a => a.GetTypes())
                     .Where(type => !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface)
                     .Where(type => IsAssignableToGenericType(type, typeof(IProjectorHandler<>))))
        {
            var serviceTypes = implementationType.GetInterfaces().Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IProjectorHandler<>));

            foreach (var serviceType in serviceTypes)
            {
                builder.Services.AddScoped(serviceType, implementationType);
            }
        }

        return builder;
    }

    private static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
        {
            return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        var baseType = givenType.BaseType;
        return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }
}
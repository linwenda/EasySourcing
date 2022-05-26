using System.Reflection;
using EasySourcing.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.DependencyInjection;

public static class EventSourcingDependencyInjection
{
    public static IServiceCollection AddEventSourcing(this IServiceCollection services,
        Action<EventSourcingBuilder> setupAction = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        services.AddScoped(typeof(IEventSourcedRepository<>), typeof(EventSourcedRepository<>));

        var builder = new EventSourcingBuilder(services);

        setupAction?.Invoke(builder);
        
        return services;
    }

    public static IServiceCollection AddProjection(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (assemblies == null || !assemblies.Any())
        {
            throw new ArgumentException(
                "No assemblies found to scan. Supply at least one assembly to scan for handlers.");
        }

        services.AddScoped<IProjector, Projector>();

        foreach (var implementationType in assemblies.SelectMany(a => a.GetTypes())
                     .Where(type => !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface)
                     .Where(type => IsAssignableToGenericType(type, typeof(IProjectorHandler<>))))
        {
            var serviceTypes = implementationType.GetInterfaces().Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IProjectorHandler<>));

            foreach (var serviceType in serviceTypes)
            {
                services.AddScoped(serviceType, implementationType);
            }
        }

        return services;
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
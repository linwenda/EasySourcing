using System.Reflection;
using EasySourcing.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.DependencyInjection;

public static class EventSourcingDependencyInjection
{
    /// <summary>
    /// Register event sourcing and handler from the specified assemblies
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assembly">Assembly to scan</param>
    /// <param name="setupAction">The action used to builder the options</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddEasySourcing(this IServiceCollection services,
        Assembly assembly,
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
        services.AddEventPublisher(new Assembly[] { assembly });
        
        var builder = new EventSourcingBuilder(services);

        setupAction?.Invoke(builder);
        
        return services;
    }
    
    /// <summary>
    /// Register event sourcing and handler from the specified assemblies
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assemblies">Assemblies to scan</param>
    /// <param name="setupAction">The action used to builder the options</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddEasySourcing(this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
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
        services.AddEventPublisher(assemblies);
        
        var builder = new EventSourcingBuilder(services);

        setupAction?.Invoke(builder);
        
        return services;
    }

    private static IServiceCollection AddEventPublisher(this IServiceCollection services, IEnumerable<Assembly> assemblies)
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

        services.AddScoped<IEventPublisher, EventPublisher>();

        foreach (var implementationType in assemblies.SelectMany(a => a.GetTypes())
                     .Where(type => !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface)
                     .Where(type => IsAssignableToGenericType(type, typeof(IEventHandler<>))))
        {
            var serviceTypes = implementationType.GetInterfaces().Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            foreach (var serviceType in serviceTypes)
            {
                services.AddTransient(serviceType, implementationType);
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
using EasySourcing.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.EntityFrameworkCore.DependencyInjection;

public static class EfCoreStoreDependencyInjection
{
    public static IEventSourcingBuilder AddEfCoreStore<TEventSourcingDbContext>(
        this IEventSourcingBuilder builder)
        where TEventSourcingDbContext : DbContext, IEventSourcingDbContext
    {
        builder.Services.AddScoped<IEventStore, EventStore<TEventSourcingDbContext>>();
        builder.Services.AddScoped<IMementoStore, MementoStore<TEventSourcingDbContext>>();

        return builder;
    }

    public static IEventSourcingBuilder UseEfCoreStore<TEventSourcingDbContext>(
        this IEventSourcingBuilder builder) where TEventSourcingDbContext : DbContext, IEventSourcingDbContext
    {
        builder.Services.AddScoped<IEventStore, EventStore<TEventSourcingDbContext>>();
        builder.Services.AddScoped<IMementoStore, MementoStore<TEventSourcingDbContext>>();

        return builder;
    }
}
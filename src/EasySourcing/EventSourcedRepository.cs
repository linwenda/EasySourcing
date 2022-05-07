using System.Globalization;
using System.Reflection;
using EasySourcing.Abstraction;
using EasySourcing.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasySourcing;

public class EventSourcedRepository<T> : IEventSourcedRepository<T> where T : EventSourced
{
    private readonly IEventStore _eventStore;
    private readonly IMementoStore _mementoStore;
    private readonly IProjector _projector;
    private readonly EventSourcingOptions _options;

    public EventSourcedRepository(
        IEventStore eventStore,
        IMementoStore mementoStore,
        IProjector projector,
        IOptions<EventSourcingOptions> optionsAccessor)
    {
        _eventStore = eventStore;
        _mementoStore = mementoStore;
        _projector = projector;
        _options = optionsAccessor?.Value ?? new EventSourcingOptions();
    }

    public async Task<T> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var eventSourced = CreateEventSourced(id);

        var minVersion = 0;

        if (eventSourced is IMementoOriginator mementoOriginator)
        {
            var memento = await _mementoStore.GetLatestMementoAsync(id, cancellationToken).ConfigureAwait(false);

            if (memento != null)
            {
                minVersion = memento.Version + 1;
                mementoOriginator.SetMemento(memento);
            }
        }

        var history = await _eventStore.LoadAsync(id, minVersion, cancellationToken).ConfigureAwait(false);

        eventSourced.LoadFrom(history);

        return eventSourced;
    }

    public async Task<T> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var eventSourced = await FindAsync(id, cancellationToken);

        if (eventSourced == null)
        {
            throw new EntityNotFoundException(typeof(T), id);
        }

        return eventSourced;
    }

    public async Task SaveAsync(T eventSourced, CancellationToken cancellationToken)
    {
        var uncommittedEvents = eventSourced.PopUncommittedEvents();

        if (!uncommittedEvents.Any()) return;

        if (eventSourced is IMementoOriginator mementoOriginator)
        {
            foreach (var uncommittedEvent in uncommittedEvents)
            {
                //Take snapshot each X version
                if (uncommittedEvent.Version >= _options.TakeEachSnapshotVersion &&
                    uncommittedEvent.Version % _options.TakeEachSnapshotVersion == 0)
                {
                    var memento = mementoOriginator.GetMemento();

                    await _mementoStore.SaveAsync(memento, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        await _eventStore.SaveAsync(uncommittedEvents, cancellationToken).ConfigureAwait(false);

        var projectorTasks = uncommittedEvents.Select(e => _projector.ProjectAsync(e, cancellationToken)).ToList();

        await Task.WhenAll(projectorTasks).ConfigureAwait(false);
    }

    private static T CreateEventSourced(Guid id)
    {
        var args = new object[] {id};

        return (T) Activator.CreateInstance(typeof(T),
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, args,
            CultureInfo.CurrentCulture);
    }
}
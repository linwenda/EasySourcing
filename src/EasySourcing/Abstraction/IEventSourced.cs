namespace EasySourcing.Abstraction;

public interface IEventSourced
{
    Guid Id { get; }
    int Version { get; }
    IReadOnlyCollection<IVersionedEvent> PopUncommittedEvents();
}
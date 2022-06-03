namespace EasySourcing.Abstraction;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent events, CancellationToken cancellationToken = default)
        where TEvent : IVersionedEvent;
}
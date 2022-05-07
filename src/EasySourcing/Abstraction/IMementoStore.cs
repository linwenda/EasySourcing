namespace EasySourcing.Abstraction;

public interface IMementoStore
{
    Task<IMemento> GetLatestMementoAsync(Guid sourcedId, CancellationToken cancellationToken = default);
    Task SaveAsync(IMemento memento, CancellationToken cancellationToken = default);
}
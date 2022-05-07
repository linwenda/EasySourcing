namespace EasySourcing.Abstraction;

public interface IMemento
{
    Guid SourcedId { get; }
    int Version { get; }
}
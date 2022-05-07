namespace EasySourcing.Abstraction
{
    public interface IVersionedEvent
    {
        public Guid SourcedId { get; }
        public int Version { get; }
    }
}
using EasySourcing.Abstraction;

namespace EasySourcing.Sample.Core.Domain;

public class PostEditedEvent : IVersionedEvent
{
    public PostEditedEvent(Guid sourcedId, int version, string title, string content)
    {
        SourcedId = sourcedId;
        Version = version;
        Title = title;
        Content = content;
    }

    public Guid SourcedId { get; }
    public int Version { get; }
    public string Title { get; }
    public string Content { get; }
}
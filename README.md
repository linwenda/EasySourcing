# EasySourcing

[![CI](https://github.com/linwenda/EasySourcing/actions/workflows/ci.yml/badge.svg)](https://github.com/linwenda/EasySourcing/actions/workflows/ci.yml)

A lightweight event sourcing implementation based on .NET 6

## Installation

You should install [EasySourcing with NuGet](https://www.nuget.org/packages/EasySourcingX):
```
Install-Package EasySourcing
```

## Quick start:

1. Create an EventSourced class

```csharp
public class Post : EventSourced
{
    private Guid _authorId;
    private string _title;
    private string _content;

    private Post(Guid id) : base(id)
    {
    }

    public static Post Initialize(Guid authorId, string title, string content)
    {
        var post = new Post(Guid.NewGuid());

        post.ApplyChange(new PostCreatedEvent(post.Id, post.GetNextVersion(), title, content, authorId));

        return post;
    }

    public void Edit(string title, string content)
    {
        ApplyChange(new PostEditedEvent(Id, GetNextVersion(), title, content));
    }

    protected override void Apply(IVersionedEvent @event)
    {
        this.When((dynamic)@event);
    }

    private void When(PostCreatedEvent @event)
    {
        _authorId = @event.AuthorId;
        _title = @event.Title;
        _content = @event.Content;
    }

    private void When(PostEditedEvent @event)
    {
        _title = @event.Title;
        _content = @event.Content;
    }
}
```

2. Pending
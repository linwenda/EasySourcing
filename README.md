# EasySourcing

[![CI](https://github.com/linwenda/EasySourcing/actions/workflows/ci.yml/badge.svg)](https://github.com/linwenda/EasySourcing/actions/workflows/ci.yml)
[![Nuget](https://img.shields.io/nuget/v/EasySourcing.EntityFrameworkCore)](https://www.nuget.org/packages/EasySourcing.EntityFrameworkCore)

A lightweight event sourcing implementation in .NET 6

## Installation

You should install [EasySourcing with NuGet](https://www.nuget.org/packages/EasySourcing.EntityFrameworkCore) (Now only EF Core implementation):
```
Install-Package EasySourcing.EntityFrameworkCore
```

## Quick start:

##### 1. Create event messages

```csharp
public class PostCreatedEvent : IVersionedEvent
{
    public PostCreatedEvent(Guid sourcedId, int version, string title, string content, Guid authorId)
    {
        SourcedId = sourcedId;
        Version = version;
        Title = title;
        Content = content;
        AuthorId = authorId;
    }

    public Guid SourcedId { get; }
    public int Version { get; }
    public string Title { get; }
    public string Content { get; }
    public Guid AuthorId { get; }
}
```
```csharp
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
```

##### 2. Create event sourced entities

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
        this.When((dynamic) @event);
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

##### 3. DI Setup

```csharp
var services = new ServiceCollection();

//You can register different database contexts
services.AddDbContext<SampleDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

services.AddEventSourcing(options =>
    {
        options.TakeEachSnapshotVersion = 5;
    })
    .AddEfCoreStore<SampleDbContext>()
    .AddProjection(typeof(PostReadModelProjector).Assembly);
```

##### 4. Use case

```csharp
public class PostService : IPostService
{
    private readonly IEventSourcedRepository<Post> _postRepository;

    public PostService(IEventSourcedRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task CreatePostAsync(Guid authorId, string title, string content)
    {
        var post = Post.Initialize(authorId, title, content);

        await _postRepository.SaveAsync(post);
    }

    public async Task EditPostAsync(Guid postId, string title, string content)
    {
        var post = await _postRepository.GetAsync(postId);

        post.Edit(title, content);

        await _postRepository.SaveAsync(post);
    }
}
```
Read model projector:

```csharp
public class PostReadModelProjector : IProjectorHandler<PostCreatedEvent>, IProjectorHandler<PostEditedEvent>
{
    private readonly PostReadModelDbContext _context;

    public PostReadModelProjector(PostReadModelDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(PostCreatedEvent @event, CancellationToken cancellationToken)
    {
        await _context.Set<PostDetail>().AddAsync(new PostDetail
        {
            Id = @event.SourcedId,
            Content = @event.Content,
            Title = @event.Title,
            AuthorId = @event.AuthorId,
            CreationTime = DateTime.UtcNow
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task HandleAsync(PostEditedEvent @event, CancellationToken cancellationToken)
    {
        var post = await _context.Set<PostDetail>().SingleAsync(p => p.Id == @event.SourcedId, cancellationToken);

        var postHistory = new PostHistory
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            AuthorId = post.AuthorId,
            Title = post.Title,
            Content = post.Content,
            CreationTime = DateTime.UtcNow
        };

        await _context.AddAsync(postHistory, cancellationToken);

        post.Title = @event.Title;
        post.Content = @event.Content;

        _context.Update(post);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
```
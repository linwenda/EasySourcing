using EasySourcing.Abstraction;
using EasySourcing.Sample.Core.Data;
using EasySourcing.Sample.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace EasySourcing.Sample.Core.ReadModels;

public class PostReadModelProjector : IProjectorHandler<PostCreatedEvent>, IProjectorHandler<PostEditedEvent>
{
    private readonly SampleDbContext _context;

    public PostReadModelProjector(SampleDbContext context)
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
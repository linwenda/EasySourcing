using System;
using System.Threading;
using System.Threading.Tasks;
using EasySourcing.Abstraction;
using EasySourcing.TestFixtures;
using Microsoft.EntityFrameworkCore;

namespace EasySourcing.EntityFrameworkCore.Tests.SeedWork;

public class PostReadModelProjector : IProjectorHandler<PostCreatedEvent>, IProjectorHandler<PostEditedEvent>
{
    private readonly TestDbContext _context;

    public PostReadModelProjector(TestDbContext context)
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

        post.Title = @event.Title;
        post.Content = @event.Content;

        _context.Update(post);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
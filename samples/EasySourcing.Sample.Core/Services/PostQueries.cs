using EasySourcing.Sample.Core.Data;
using EasySourcing.Sample.Core.Interfaces;
using EasySourcing.Sample.Core.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace EasySourcing.Sample.Core.Services;

public class PostQueries : IPostQueries
{
    private readonly SampleDbContext _context;

    public PostQueries(SampleDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PostDetail>> GetPostsAsync()
    {
        return await _context.Set<PostDetail>().ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<PostHistory>> GetPostHistoriesAsync(Guid postId)
    {
        return await _context.Set<PostHistory>().Where(p => p.PostId == postId)
            .OrderByDescending(p => p.CreationTime)
            .ToListAsync().ConfigureAwait(false);
    }
}
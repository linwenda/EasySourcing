using System;
using System.Linq;
using System.Threading.Tasks;
using EasySourcing.Abstraction;
using EasySourcing.EntityFrameworkCore.Tests.SeedWork;
using EasySourcing.TestFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasySourcing.EntityFrameworkCore.Tests;

public class EventSourcedRepositoryTests : TestBase
{
    private readonly TestDbContext _context;
    private readonly IEventSourcedRepository<Post> _postRepository;
    private readonly IMementoStore _mementoStore;

    public EventSourcedRepositoryTests()
    {
        _context = ServiceProvider.GetRequiredService<TestDbContext>();
        _postRepository = ServiceProvider.GetRequiredService<IEventSourcedRepository<Post>>();
        _mementoStore = ServiceProvider.GetRequiredService<IMementoStore>();
    }

    [Fact]
    public async Task CanSavePostEventsAndGenerateReadModel()
    {
        var post = Post.Initialize(Guid.NewGuid(), "test", "test");

        await _postRepository.SaveAsync(post);

        var postReadModel = await _context.Set<PostDetail>().SingleAsync(p => p.Id == post.Id);
        Assert.Equal("test", postReadModel.Title);
        Assert.Equal("test", postReadModel.Content);
    }

    [Fact]
    public async Task CanLoadEventSourcedByHistory()
    {
        var post = Post.Initialize(Guid.NewGuid(), "test", "test");

        post.Edit("test2", "test2");

        await _postRepository.SaveAsync(post);

        var postFromHistory = await _postRepository.FindAsync(post.Id);
        Assert.Equal(2, postFromHistory.Version);
    }

    [Fact]
    public async Task CanTakeEventSourcedSnapshot()
    {
        var post = Post.Initialize(Guid.NewGuid(), "test", "test");

        post.Edit("test2", "test2");
        await _postRepository.SaveAsync(post);

        post.Edit("test3", "test3"); //will take snapshot
        await _postRepository.SaveAsync(post);
        
        post.Edit("test4", "test4");
        await _postRepository.SaveAsync(post);

        var postMemento = await _mementoStore.GetLatestMementoAsync(post.Id) as PostMemento;

        Assert.NotNull(postMemento);
        Assert.Equal(3, postMemento.Version);

        var postFromMemento = await _postRepository.FindAsync(post.Id);
        Assert.Equal(4, postFromMemento.Version);
    }
}
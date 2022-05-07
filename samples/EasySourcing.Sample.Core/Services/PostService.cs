using EasySourcing.Abstraction;
using EasySourcing.Sample.Core.Domain;
using EasySourcing.Sample.Core.Interfaces;

namespace EasySourcing.Sample.Core.Services;

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
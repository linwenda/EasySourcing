namespace EasySourcing.Sample.Core.Interfaces;

public interface IPostService
{
    Task CreatePostAsync(Guid authorId, string title, string content);
    Task EditPostAsync(Guid postId, string title, string content);
}
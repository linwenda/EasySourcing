using EasySourcing.Sample.Core.ReadModels;

namespace EasySourcing.Sample.Core.Interfaces;

public interface IPostQueries
{
    Task<IEnumerable<PostDetail>> GetPostsAsync();
    Task<IEnumerable<PostHistory>> GetPostHistoriesAsync(Guid postId);
}
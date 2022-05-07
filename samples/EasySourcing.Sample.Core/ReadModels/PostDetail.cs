namespace EasySourcing.Sample.Core.ReadModels;

//ReadModel
public class PostDetail
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
namespace EasySourcing.Sample.Api.Controllers;

public class CreatePostModel
{
    public Guid AuthorId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
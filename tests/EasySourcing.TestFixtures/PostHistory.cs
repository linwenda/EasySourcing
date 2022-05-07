using System;

namespace EasySourcing.EntityFrameworkCore.Tests;

public class PostHistory
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
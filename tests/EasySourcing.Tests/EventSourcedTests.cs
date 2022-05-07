using System;
using EasySourcing.TestFixtures;
using Xunit;

namespace EasySourcing.UnitTests;

public class EventSourcedTests
{
    [Fact]
    public void CanApplyChangeEvents()
    {
        var input = new
        {
            AuthorId = Guid.NewGuid(),
            InitTitle = ".NET 6",
            InitContent = ".NET 6 new feature",
            EditTitle = ".NET 7",
            EditContent = ".NET 7 new feature",
        };

        var post = Post.Initialize(input.AuthorId, input.InitTitle, input.InitContent);
        var postCreatedEvent = post.PopUncommittedEvents().Have<PostCreatedEvent>();

        Assert.Equal(input.AuthorId, postCreatedEvent.AuthorId);
        Assert.Equal(input.InitTitle, postCreatedEvent.Title);
        Assert.Equal(input.InitContent, postCreatedEvent.Content);
        Assert.Equal(1, postCreatedEvent.Version);

        post.Edit(input.EditTitle,input.EditContent);
        var postEditedEvent = post.PopUncommittedEvents().Have<PostEditedEvent>();

        Assert.Equal(input.EditTitle, postEditedEvent.Title);
        Assert.Equal(input.EditContent, postEditedEvent.Content);
        Assert.Equal(2, postEditedEvent.Version);
    }
}
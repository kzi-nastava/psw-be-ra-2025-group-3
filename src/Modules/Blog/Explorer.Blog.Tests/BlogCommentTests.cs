using Explorer.Blog.Core.Domain.Blogs;
using Xunit;

public class BlogCommentTests
{
    [Fact]
    public void Can_add_comment_to_published_blog()
    {
        // arrange
        var blog = new Blog("Title", "Description", authorId: 1);
        blog.ChangeStatus(1); // Published

        // act
        blog.AddComment(userId: 2, text: "Test comment");

        // assert
        Assert.Single(blog.Comments);
        Assert.Equal("Test comment", blog.Comments[0].Text);
    }

    [Fact]
    public void Can_add_comment_to_draft_blog_current_behavior()
    {
        var blog = new Blog("Title", "Description", authorId: 1);

        blog.AddComment(userId: 2, text: "Draft comment");

        Assert.Single(blog.Comments);
    }


    [Fact]
    public void Author_can_edit_comment_within_15_minutes()
    {
        var blog = new Blog("Title", "Description", authorId: 1);
        blog.ChangeStatus(1);

        blog.AddComment(userId: 2, text: "Initial");
        var comment = blog.Comments[0];

        blog.EditComment(comment.Id, userId: 2, newText: "Edited");

        Assert.Equal("Edited", comment.Text);
        Assert.NotNull(comment.EditedAt);
    }

    [Fact]
    public void Author_can_delete_own_comment()
    {
        var blog = new Blog("Title", "Description", authorId: 1);
        blog.ChangeStatus(1);

        blog.AddComment(userId: 2, text: "To delete");
        var commentId = blog.Comments[0].Id;

        blog.DeleteComment(commentId, userId: 2);

        Assert.Empty(blog.Comments);
    }
}

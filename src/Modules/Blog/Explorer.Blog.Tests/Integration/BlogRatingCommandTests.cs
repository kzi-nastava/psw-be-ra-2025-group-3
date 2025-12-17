using Explorer.API.Controllers.Author_Tourist;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Explorer.Blog.Tests.Integration
{
    [Collection("Sequential")]
    public class BlogVoteCommandTests : BaseBlogIntegrationTest
    {
        public BlogVoteCommandTests(BlogTestFactory factory) : base(factory) { }

        private void AttachUser(BlogController controller, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("id", userId.ToString()),
                new Claim(ClaimTypes.Role, "author")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private static BlogController CreateController(IServiceScope scope)
        {
            return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>());
        }

        [Fact]
        public void Vote_adds_upvote_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Arrange - uloguj korisnika i kreiraj blog
            var userId = -11;
            AttachUser(controller, userId);

            var createDto = new BlogDto
            {
                Title = "Blog za glasanje - upvote",
                Description = "Opis bloga za testiranje upvote-a"
            };

            var createResult = controller.CreateBlog(createDto);
            createResult.Result.ShouldBeOfType<CreatedAtActionResult>();

            var createdBlog = (createResult.Result as CreatedAtActionResult)!.Value as BlogDto;
            createdBlog.ShouldNotBeNull();
            var blogId = createdBlog.Id;

            var publishResult = controller.ChangeStatus(blogId, newStatus: 1);
            publishResult.Result.ShouldBeOfType<OkObjectResult>();

            // Act - korisnik daje upvote
            var voteDto = new BlogVoteDto
            {
                BlogId = blogId,
                IsUpvote = true
            };

            var voteResult = controller.Vote(blogId, voteDto);
            voteResult.Result.ShouldBeOfType<OkObjectResult>();

            var okVoteResult = voteResult.Result as OkObjectResult;
            var voteState = okVoteResult!.Value as BlogVoteStateDto;

            // Assert
            voteState.ShouldNotBeNull();
            voteState.BlogId.ShouldBe(blogId);
            voteState.IsUpvote.ShouldBe(true);
            voteState.UpvoteCount.ShouldBe(1);
            voteState.DownvoteCount.ShouldBe(0);
            voteState.Score.ShouldBe(1);
        }

        [Fact]
        public void Vote_toggles_off_when_same_vote_clicked_again()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var userId = -11;
            AttachUser(controller, userId);

            // Kreiraj blog
            var createDto = new BlogDto
            {
                Title = "Blog za toggle glasanja",
                Description = "Opis"
            };

            var createResult = controller.CreateBlog(createDto);
            var createdBlog = (createResult.Result as CreatedAtActionResult)!.Value as BlogDto;
            createdBlog.ShouldNotBeNull();
            var blogId = createdBlog.Id;

            var publishResult = controller.ChangeStatus(blogId, newStatus: 1); // Objavi blog
            publishResult.Result.ShouldBeOfType<OkObjectResult>();

            // Prvi put: upvote
            var voteDto = new BlogVoteDto { BlogId = blogId, IsUpvote = true };
            controller.Vote(blogId, voteDto);

            // Drugi put: isti upvote -> treba da ukloni glas
            var toggleResult = controller.Vote(blogId, voteDto);
            toggleResult.Result.ShouldBeOfType<OkObjectResult>();

            var okToggle = toggleResult.Result as OkObjectResult;
            var voteState = okToggle!.Value as BlogVoteStateDto;

            // Assert
            voteState.ShouldNotBeNull();
            voteState.IsUpvote.ShouldBeNull();
            voteState.UpvoteCount.ShouldBe(0);
            voteState.DownvoteCount.ShouldBe(0);
            voteState.Score.ShouldBe(0);
        }

        [Fact]
        public void Vote_changes_from_upvote_to_downvote()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var userId = -11;
            AttachUser(controller, userId);

            // Kreiraj blog
            var createDto = new BlogDto
            {
                Title = "Blog za promenu glasa",
                Description = "Opis"
            };

            var createResult = controller.CreateBlog(createDto);
            var createdBlog = (createResult.Result as CreatedAtActionResult)!.Value as BlogDto;
            createdBlog.ShouldNotBeNull();
            var blogId = createdBlog.Id;

            var publishResult = controller.ChangeStatus(blogId, newStatus: 1);
            publishResult.Result.ShouldBeOfType<OkObjectResult>();

            // Prvo: upvote
            var upvoteDto = new BlogVoteDto { BlogId = blogId, IsUpvote = true };
            controller.Vote(blogId, upvoteDto);

            // Zatim: downvote
            var downvoteDto = new BlogVoteDto { BlogId = blogId, IsUpvote = false };
            var changeResult = controller.Vote(blogId, downvoteDto);

            changeResult.Result.ShouldBeOfType<OkObjectResult>();

            var okChange = changeResult.Result as OkObjectResult;
            var voteState = okChange!.Value as BlogVoteStateDto;

            // Assert
            voteState.ShouldNotBeNull();
            voteState.IsUpvote.ShouldBe(false);
            voteState.UpvoteCount.ShouldBe(0);
            voteState.DownvoteCount.ShouldBe(1);
            voteState.Score.ShouldBe(-1);
        }

        [Fact]
        public void Vote_aggregates_multiple_users_votes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // User -11 kreira blog
            AttachUser(controller, -11);

            var createDto = new BlogDto
            {
                Title = "Blog za više korisnika",
                Description = "Opis"
            };

            var createResult = controller.CreateBlog(createDto);
            var createdBlog = (createResult.Result as CreatedAtActionResult)!.Value as BlogDto;
            createdBlog.ShouldNotBeNull();
            var blogId = createdBlog.Id;

            var publishResult = controller.ChangeStatus(blogId, newStatus: 1);
            publishResult.Result.ShouldBeOfType<OkObjectResult>();

            // User -11: upvote
            AttachUser(controller, -11);
            var voteDto1 = new BlogVoteDto { BlogId = blogId, IsUpvote = true };
            controller.Vote(blogId, voteDto1);

            // User -12: downvote
            AttachUser(controller, -12);
            var voteDto2 = new BlogVoteDto { BlogId = blogId, IsUpvote = false };
            controller.Vote(blogId, voteDto2);

            // User -13: upvote
            AttachUser(controller, -13);
            var voteDto3 = new BlogVoteDto { BlogId = blogId, IsUpvote = true };
            var finalResult = controller.Vote(blogId, voteDto3);

            finalResult.Result.ShouldBeOfType<OkObjectResult>();

            var okFinal = finalResult.Result as OkObjectResult;
            var voteState = okFinal!.Value as BlogVoteStateDto;

            // Assert
            voteState.ShouldNotBeNull();
            voteState.UpvoteCount.ShouldBe(2);
            voteState.DownvoteCount.ShouldBe(1);
            voteState.Score.ShouldBe(1); // +1 -1 +1 = 1
        }
    }
}
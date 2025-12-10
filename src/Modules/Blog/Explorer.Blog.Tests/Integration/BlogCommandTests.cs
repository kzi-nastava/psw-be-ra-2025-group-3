using Explorer.API.Controllers.Author_Tourist;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Explorer.Blog.Tests.Integration
{
    [Collection("Sequential")]
    public class BlogCommandTests : BaseBlogIntegrationTest
    {
        public BlogCommandTests(BlogTestFactory factory) : base(factory) { }

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

        [Fact]
        public void Create_fails_invalid_data()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            AttachUser(controller, -11);

            //  Pošto vaš kontroler NE proverava ModelState,
            // a Blog konstruktor baca exception za prazan Title,
            // test mora da očekuje da će biti bacen ArgumentException
            var invalid = new BlogDto
            {
                Title = "",  // Prazan title će baciti exception u Blog konstruktoru
                Description = "Valid description"
            };

            //  Očekuj ArgumentException sa porukom "Title cannot be empty"
            var exception = Should.Throw<ArgumentException>(() =>
            {
                var result = controller.CreateBlog(invalid);
            });

            exception.Message.ShouldBe("Title cannot be empty");
        }

        [Fact]
        public void Creates_blog_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            AttachUser(controller, -11);

            var dto = new BlogDto
            {
                Title = "Test blog for creation",
                Description = "Opis bloga za testiranje kreiranja",
                Images = new List<BlogImageDto>
                {
                    new BlogImageDto { ImageUrl = "https://example.com/test-create.jpg" }
                }
            };

            var result = controller.CreateBlog(dto);

            result.Result.ShouldBeOfType<CreatedAtActionResult>();

            var createdResult = result.Result as CreatedAtActionResult;
            var createdBlog = createdResult.Value as BlogDto;

            createdBlog.ShouldNotBeNull();
            createdBlog.Title.ShouldBe(dto.Title);
            createdBlog.Description.ShouldBe(dto.Description);
            createdBlog.AuthorId.ShouldBe(-11);
            createdBlog.Id.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            AttachUser(controller, -11);

            var updated = new BlogDto
            {
                Id = -999, // Blog koji ne postoji
                Title = "Updated Title",
                Description = "Updated Description"
            };

            var result = controller.UpdateBlog(-999, updated);

            // Kontroler vraća Forbid() kada ne nađe blog u GetUserBlogs
            result.Result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Update_fails_forbidden()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Pokušavamo da update-ujemo blog koji pripada korisniku -11
            // ali smo ulogovani kao korisnik -22
            AttachUser(controller, -22);

            var updated = new BlogDto
            {
                Id = -1, // Ovaj blog pripada korisniku -11 (iz seed-a)
                Title = "Pokušaj izmene",
                Description = "Opis izmene"
            };

            var result = controller.UpdateBlog(-1, updated);

            // Trebalo bi da dobijemo Forbid jer blog ne pripada korisniku -22
            result.Result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Updates_blog_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            AttachUser(controller, -11);

            //  KLJUČNA ISPRAVKA: Eksplicitno postavi Id = -1
            var updated = new BlogDto
            {
                Id = -1, //  MORA biti eksplicitno postavljen!
                Title = "Ažurirani naslov",
                Description = "Ažuriran opis bloga",
                AuthorId = -11, //  Eksplicitno postavi AuthorId
                Images = new List<BlogImageDto>
                {
                    new BlogImageDto
                    {
                        ImageUrl = "https://example.com/new-image.jpg",
                        BlogId = -1 //  Postavi BlogId za slike
                    }
                }
            };

            var result = controller.UpdateBlog(-1, updated);

            result.Result.ShouldBeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            var updatedBlog = okResult.Value as BlogDto;

            updatedBlog.ShouldNotBeNull();
            updatedBlog.Id.ShouldBe(-1);
            updatedBlog.Title.ShouldBe(updated.Title);
            updatedBlog.Description.ShouldBe(updated.Description);
            updatedBlog.AuthorId.ShouldBe(-11);
        }

        [Fact]
        public void GetUserBlogs_returns_only_user_blogs()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            AttachUser(controller, -11);
            controller.CreateBlog(new BlogDto
            {
                Title = "Planinarenje u Alpima",
                Description = "Opis"
            });
            controller.CreateBlog(new BlogDto
            {
                Title = "Kulinarsko putovanje kroz Italiju",
                Description = "Opis"
            });

            AttachUser(controller, -12);
            controller.CreateBlog(new BlogDto
            {
                Title = "Fotografija pejzaža",
                Description = "Opis"
            });

            AttachUser(controller, -11);
            var result = controller.GetUserBlogs();

            result.Result.ShouldBeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            var blogs = okResult.Value as List<BlogDto>;

            blogs.ShouldNotBeNull();

            blogs.ShouldAllBe(b => b.AuthorId == -11);
            blogs.ShouldContain(b => b.Title == "Planinarenje u Alpima");
            blogs.ShouldContain(b => b.Title == "Kulinarsko putovanje kroz Italiju");
        }

        [Fact]
        public void UpdateBlog_allows_full_update_for_draft()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            AttachUser(controller, -11);

            var updateDto = new BlogDto
            {
                Id = -1,
                Title = "Novi naslov",
                Description = "Novi opis",
                Images = new List<BlogImageDto> { new BlogImageDto { ImageUrl = "img.jpg" } }
            };

            var result = controller.UpdateBlog(-1, updateDto);

            result.Result.ShouldBeOfType<OkObjectResult>();
        }

        private static BlogController CreateController(IServiceScope scope)
        {
            return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>());
        }
    }
}
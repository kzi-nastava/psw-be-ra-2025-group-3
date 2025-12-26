using Explorer.Blog.API.Dtos;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Explorer.Blog.Tests.Integration
{
    public class NewsletterCommandTests : BaseTestFactory<BlogContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            services.RemoveAll(typeof(BlogContext));
            services.AddDbContext<BlogContext>(SetupTestContext());
            return services;
        }

        [Fact]
        public async Task Subscribe_Should_Return_Ok_For_Valid_Email()
        {
            // arrange
            var client = CreateClient();

            var dto = new NewsletterSubscriptionDto
            {
                Email = "newsletter.valid@gmail.com"
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // act
            var response = await client.PostAsync("/api/blog/newsletter", content);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Subscribe_Should_Return_Conflict_For_Duplicate_Email()
        {
            // arrange
            var client = CreateClient();

            var dto = new NewsletterSubscriptionDto
            {
                Email = "duplicate.newsletter@gmail.com"
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // first request
            await client.PostAsync("/api/blog/newsletter", content);

            // second request (duplicate)
            var response = await client.PostAsync("/api/blog/newsletter", content);

            // assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Subscribe_Should_Return_BadRequest_For_Invalid_Email()
        {
            // arrange
            var client = CreateClient();

            var dto = new NewsletterSubscriptionDto
            {
                Email = "ema@"
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // act
            var response = await client.PostAsync("/api/blog/newsletter", content);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

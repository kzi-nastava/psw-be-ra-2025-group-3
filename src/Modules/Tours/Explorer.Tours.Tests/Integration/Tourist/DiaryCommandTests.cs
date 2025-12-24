using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class DiaryCommandTests : BaseToursIntegrationTest
    {
        public DiaryCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_diary_for_tourist()
        {
            // Arrange
            var personId = "-21";

            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var request = new DiaryCreateDto
            {
                Title = "Test diary",
                Country = "Serbia",
                City = "Novi Sad"
            };

            // Act
            var result = ((ObjectResult)controller.Create(request).Result)?.Value as DiaryDto;

            // Assert – response
            result.ShouldNotBeNull();
            result.Title.ShouldBe("Test diary");
            result.Country.ShouldBe("Serbia");
            result.City.ShouldBe("Novi Sad");
            result.Status.ShouldBe(0); // Draft
            result.TouristId.ShouldBe(int.Parse(personId));

            // Assert – database
            var storedDiary = dbContext.Diaries.FirstOrDefault(d => d.Id == result.Id);
            storedDiary.ShouldNotBeNull();
            storedDiary.Title.ShouldBe("Test diary");
            ((int)storedDiary.Status).ShouldBe(0);
        }

        private static DiaryController CreateController(IServiceScope scope, string personId)
        {
            return new DiaryController(scope.ServiceProvider.GetRequiredService<IDiaryService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}

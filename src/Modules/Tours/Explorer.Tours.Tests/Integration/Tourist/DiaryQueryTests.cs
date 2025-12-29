using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class DiaryQueryTests : BaseToursIntegrationTest
    {
        public DiaryQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Get_returns_only_my_diaries()
        {
            // Arrange
            var personId = "-22";

            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId);

            controller.Create(new DiaryCreateDto
            {
                Title = "My diary",
                Country = "Italy",
                City = "Rome"
            });

            // Act
            var result = ((ObjectResult)controller.GetMyDiaries().Result)?.Value as List<DiaryDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.All(d => d.TouristId == int.Parse(personId)).ShouldBeTrue();
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

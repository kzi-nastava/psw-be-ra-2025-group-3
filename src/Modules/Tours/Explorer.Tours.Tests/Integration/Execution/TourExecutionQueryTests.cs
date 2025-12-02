using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Execution;

[Collection("Sequential")]
public class TourExecutionQueryTests : BaseToursIntegrationTest
{
    public TourExecutionQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Checks_no_active_session_initially()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITourExecutionRepository>();

        // Act
        var hasSession = repository.HasActiveSession(-23, -2);

        // Assert
        hasSession.ShouldBeFalse();
    }
}
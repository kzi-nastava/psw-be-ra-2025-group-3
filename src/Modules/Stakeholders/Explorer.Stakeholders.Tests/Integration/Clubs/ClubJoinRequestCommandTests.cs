using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs
{
    [Collection("Sequential")]
    public class ClubJoinRequestCommandTests : BaseStakeholdersIntegrationTest
    {
        public ClubJoinRequestCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Send_request_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IClubJoinRequestService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            long touristId = -24;
            long clubId = -1; 

            // Act
            var result = service.Send(touristId, clubId);

            // Assert
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(touristId);
            result.ClubId.ShouldBe(clubId);

            // Provera u bazi
            var requestInDb = dbContext.ClubJoinRequests.FirstOrDefault(r => r.TouristId == touristId && r.ClubId == clubId);
            requestInDb.ShouldNotBeNull();
        }

        [Fact]
        public void Respond_accept_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IClubJoinRequestService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            long touristId = -24;
            long clubId = -2;   
            long ownerId = -22; 

            // Prvo kreiramo zahtev rucno
            var request = new ClubJoinRequest(touristId, clubId);
            dbContext.ClubJoinRequests.Add(request);
            dbContext.SaveChanges();

            // Act
            service.Respond(ownerId, request.Id, true);

            // Assert
            // 1. Zahtev treba da bude obrisan
            var requestInDb = dbContext.ClubJoinRequests.FirstOrDefault(r => r.Id == request.Id);
            requestInDb.ShouldBeNull();

            // 2. Turista treba da bude u listi clanova kluba
            var club = dbContext.Clubs.FirstOrDefault(c => c.Id == clubId);
            club.MemberIds.ShouldContain(touristId);
        }

        [Fact]
        public void Respond_reject_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IClubJoinRequestService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            long touristId = -24;
            long clubId = -3;  
            long ownerId = -23;

            // Kreiramo zahtev
            var request = new ClubJoinRequest(touristId, clubId);
            dbContext.ClubJoinRequests.Add(request);
            dbContext.SaveChanges();

            // Act
            service.Respond(ownerId, request.Id, false);

            // Assert
            // 1. Zahtev treba da bude obrisan
            var requestInDb = dbContext.ClubJoinRequests.FirstOrDefault(r => r.Id == request.Id);
            requestInDb.ShouldBeNull();

            // 2. Turista NE SME da bude u listi clanova kluba
            var club = dbContext.Clubs.FirstOrDefault(c => c.Id == clubId);
            club.MemberIds.ShouldNotContain(touristId);
        }
    }
}
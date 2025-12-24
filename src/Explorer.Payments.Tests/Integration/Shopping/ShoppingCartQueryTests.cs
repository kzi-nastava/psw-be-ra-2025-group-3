using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Explorer.API.Controllers.Shopping;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Moq;
using Explorer.Payments.Core.UseCases.Shopping;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Infrastructure.Database.Repositories;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Explorer.Payments.Core.Mappers;

namespace Explorer.Payments.Tests.Integration.Shopping
{
    [Collection("Sequential")]
    public class ShoppingCartQueryTests : BasePaymentsIntegrationTest
    {
        public ShoppingCartQueryTests(PaymentsTestFactory factory) : base(factory) { }
        
        [Fact]
        public void Get_returns_current_cart()
        {
            // Arrange
            var personId = "-22";
            var tourId = -2;
            var expectedPrice = 500m;

            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var addRequest = new ShoppingCartRequestDto
            {
                TourId = tourId
            };
            dbContext.ShoppingCarts.RemoveRange(dbContext.ShoppingCarts);
            dbContext.SaveChanges();
            controller.Add(addRequest);

            var result = ((ObjectResult)controller.Get().Result)?.Value as ShoppingCartDto;

            // Assert 
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(long.Parse(personId));
            result.Items.Count.ShouldBe(1);
            result.Items[0].TourId.ShouldBe(tourId);
            result.Items[0].Price.ShouldBe(expectedPrice);
            result.TotalPrice.ShouldBe(expectedPrice);

            // Assert 
            var storedCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.TouristId == long.Parse(personId));
            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(1);
            storedCart.Items[0].TourId.ShouldBe(tourId);
            storedCart.TotalPrice.ShouldBe(expectedPrice);
        }

        /*[Fact]
        public void Get_returns_current_cart()
        {
            var personId = "-22";
            var tourId = -10;
            var expectedPrice = 500m;

            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Mock internal tour
            var internalTourMock = new Mock<IInternalTourService>();
            internalTourMock.Setup(s => s.GetById(tourId)).Returns(new TourDto
            {
                Id = tourId,
                Name = "Test Tour",
                Price = expectedPrice,
                Status = (int)TourStatusDto.Published,
                ArchivedAt = null
            });

            // Build mapper with PaymentProfile
            var mapper = new MapperConfiguration(cfg =>
                cfg.AddMaps(typeof(PaymentProfile).Assembly)
            ).CreateMapper();

            // Use real repo + in-memory DB
            var cartRepo = scope.ServiceProvider.GetRequiredService<IShoppingCartRepository>();
            var svc = new ShoppingCartService(cartRepo, internalTourMock.Object, mapper);

            var controller = new ShoppingCartController(svc)
            {
                ControllerContext = BuildContext(personId)
            };

            // Act
            controller.Add(new Explorer.Payments.API.Dtos.ShoppingCartRequestDto { TourId = tourId });
            var result = ((ObjectResult)controller.Get().Result)?.Value as ShoppingCartDto;

            // Assert DTO
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(long.Parse(personId));
            result.Items.Count.ShouldBe(1);
            result.Items[0].TourId.ShouldBe(tourId);
            result.Items[0].Price.ShouldBe(expectedPrice);
            result.TotalPrice.ShouldBe(expectedPrice);

            // Assert DB
            var stored = dbContext.ShoppingCarts.FirstOrDefault(c => c.TouristId == long.Parse(personId));
            stored.ShouldNotBeNull();
            stored.Items.Count.ShouldBe(1);
            stored.Items[0].TourId.ShouldBe(tourId);
            stored.TotalPrice.ShouldBe(expectedPrice);
        }*/

        private static ShoppingCartController CreateController(IServiceScope scope, string personId)
        {
            return new ShoppingCartController(scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}


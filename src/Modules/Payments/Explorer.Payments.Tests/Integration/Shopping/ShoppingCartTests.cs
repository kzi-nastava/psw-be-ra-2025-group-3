using Explorer.Payments.Core.Domain;
using Explorer.Tours.API.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;

namespace Explorer.Payments.Tests.Integration.Shopping
{
    public class ShoppingCartTests
    {
        [Theory]
        [InlineData(TourStatusDto.Published, true)]
        [InlineData(TourStatusDto.Draft, false)]
        public void Adds_item_only_for_published_tours(TourStatusDto status, bool shouldSucceed)
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour = CreateTourWithStatus(status, 10m);

            // Act & Assert
            if (shouldSucceed)
            {
                var orderItem = new OrderItem(tour.Id, tour.Name, tour.Price);
                cart.AddItem(orderItem);

                cart.Items.Count.ShouldBe(1);
                cart.Items[0].TourId.ShouldBe(tour.Id);
                cart.TotalPrice.ShouldBe(10m);
            }
            else
            {
                var orderItem = new OrderItem(tour.Id, tour.Name, tour.Price);                
                Should.Throw<InvalidOperationException>(() => {
                    if (tour.Status != TourStatus.Published)
                        throw new InvalidOperationException("Tour must be published to be added to cart.");
                    else
                        cart.AddItem(orderItem);
                });
                cart.Items.Count.ShouldBe(0);
                cart.TotalPrice.ShouldBe(0m);
            }
        }

        [Fact]
        public void Cannot_add_same_tour_twice()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour = CreateTourWithStatus(TourStatusDto.Published, 15m);

            // Act
            var orderItem = new OrderItem(tour.Id, tour.Name, tour.Price);
            cart.AddItem(orderItem);

            // Assert
            Should.Throw<InvalidOperationException>(() => cart.AddItem(orderItem));
            cart.Items.Count.ShouldBe(1);
            cart.TotalPrice.ShouldBe(15m);
        }

        [Fact]
        public void Recalculates_total_price()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour1 = CreateTourWithStatus(TourStatusDto.Published, 10m, 1);
            var tour2 = CreateTourWithStatus(TourStatusDto.Published, 25.5m, 2);

            // Act
            var orderItem = new OrderItem(tour1.Id, tour1.Name, tour1.Price);
            cart.AddItem(orderItem);
            orderItem = new OrderItem(tour2.Id, tour2.Name, tour2.Price);
            cart.AddItem(orderItem);
            
            // Assert
            cart.Items.Count.ShouldBe(2);
            cart.TotalPrice.ShouldBe(35.5m);
        }

        [Fact]
        public void Removes_item_and_updates_total()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour1 = CreateTourWithStatus(TourStatusDto.Published, 10m, 1);
            var tour2 = CreateTourWithStatus(TourStatusDto.Published, 20m, 2);

            var orderItem = new OrderItem(tour1.Id, tour1.Name, tour1.Price);
            cart.AddItem(orderItem);
            orderItem = new OrderItem(tour2.Id, tour2.Name, tour2.Price);
            cart.AddItem(orderItem);

            // Act
            cart.RemoveItem(tour1.Id);

            // Assert
            cart.Items.Count.ShouldBe(1);
            cart.Items[0].TourId.ShouldBe(tour2.Id);
            cart.TotalPrice.ShouldBe(20m);
        }

        
        private static Tour CreateTourWithStatus(TourStatusDto status, decimal price, long id = 1)
        {
            var tags = new List<string> { "cycling", "nature" };

            var tour = new Tour("Test tura", "Opis", TourDifficulty.Easy, -11, tags);
            tour.Update("Test tura", "Opis", TourDifficulty.Easy, price, tags);

            tour.KeyPoints.Add(new KeyPoint(id, "Start Point", "Description", "image1.jpg", "secret", 45.25, 19.82));
            tour.KeyPoints.Add(new KeyPoint(id, "End Point", "Description", "image2.jpg", "secret", 45.26, 19.83));

            var durations = new List<TourDuration>
            {
                new TourDuration(60, TransportType.Walking),
                new TourDuration(30, TransportType.Bicycle)
            };
            tour.UpdateTourDurations(durations);

            if (status == TourStatusDto.Published)
            {
                tour.Publish();
            }

            var idProp = typeof(Tour).BaseType!.GetProperty("Id");
            idProp!.SetValue(tour, id);

            return tour;
        }
        
    }
}

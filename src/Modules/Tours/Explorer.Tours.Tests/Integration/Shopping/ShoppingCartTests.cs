using Explorer.Tours.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Shopping
{
    public class ShoppingCartTests
    {
        [Theory]
        [InlineData(TourStatus.Published, true)]
        [InlineData(TourStatus.Draft, false)]
        public void Adds_item_only_for_published_tours(TourStatus status, bool shouldSucceed)
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour = CreateTourWithStatus(status, 10m);

            // Act & Assert
            if (shouldSucceed)
            {
                cart.AddItem(tour);

                cart.Items.Count.ShouldBe(1);
                cart.Items[0].TourId.ShouldBe(tour.Id);
                cart.TotalPrice.ShouldBe(10m);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() => cart.AddItem(tour));
                cart.Items.Count.ShouldBe(0);
                cart.TotalPrice.ShouldBe(0m);
            }
        }

        [Fact]
        public void Cannot_add_same_tour_twice()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour = CreateTourWithStatus(TourStatus.Published, 15m);

            // Act
            cart.AddItem(tour);

            // Assert
            Should.Throw<InvalidOperationException>(() => cart.AddItem(tour));
            cart.Items.Count.ShouldBe(1);
            cart.TotalPrice.ShouldBe(15m);
        }

        [Fact]
        public void Recalculates_total_price()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour1 = CreateTourWithStatus(TourStatus.Published, 10m, 1);
            var tour2 = CreateTourWithStatus(TourStatus.Published, 25.5m, 2);

            // Act
            cart.AddItem(tour1);
            cart.AddItem(tour2);

            // Assert
            cart.Items.Count.ShouldBe(2);
            cart.TotalPrice.ShouldBe(35.5m);
        }

        [Fact]
        public void Removes_item_and_updates_total()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            var tour1 = CreateTourWithStatus(TourStatus.Published, 10m, 1);
            var tour2 = CreateTourWithStatus(TourStatus.Published, 20m, 2);

            cart.AddItem(tour1);
            cart.AddItem(tour2);

            // Act
            cart.RemoveItem(tour1.Id);

            // Assert
            cart.Items.Count.ShouldBe(1);
            cart.Items[0].TourId.ShouldBe(tour2.Id);
            cart.TotalPrice.ShouldBe(20m);
        }

        private static Tour CreateTourWithStatus(TourStatus status, decimal price, long id = 1)
        {
            var tour = new Tour("Test tura", "Opis", TourDifficulty.Easy, -11);
            tour.Update("Test tura", "Opis", TourDifficulty.Easy, price, null);

            if (status == TourStatus.Published)
            {
                tour.TemporaryPublish();
            }

            var idProp = typeof(Tour).BaseType!.GetProperty("Id");
            idProp!.SetValue(tour, id);

            return tour;
        }
    }
}

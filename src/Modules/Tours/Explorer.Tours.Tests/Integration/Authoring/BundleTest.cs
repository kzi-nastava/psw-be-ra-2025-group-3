using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring
{
    [Collection("Sequential")]
    public class BundleTests : BaseToursIntegrationTest
    {
        public BundleTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_bundle()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var bundleDto = new BundleCreateDto
            {
                Name = "Summer Adventure Bundle",
                Price = 1500,
                TourIds = new List<long> { -2, -4 } // Published tours
            };

            // Act
            var result = service.Create(bundleDto, -11);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Summer Adventure Bundle");
            result.Price.ShouldBe(1500);
            result.Status.ShouldBe(0); // Draft
            result.TourIds.Count.ShouldBe(2);
        }

        [Fact]
        public void Creates_bundle_fails_tour_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var bundleDto = new BundleCreateDto
            {
                Name = "Invalid Bundle",
                Price = 1000,
                TourIds = new List<long> { -2, 999 } // 999 doesn't exist
            };

            // Act & Assert
            Should.Throw<Exception>(() => service.Create(bundleDto, -11));
        }

        [Fact]
        public void Creates_bundle_fails_tour_not_owned()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var bundleDto = new BundleCreateDto
            {
                Name = "Wrong Owner Bundle",
                Price = 1000,
                TourIds = new List<long> { -2 } // This tour belongs to author -11
            };

            // Act & Assert
            Should.Throw<Exception>(() => service.Create(bundleDto, -12)); // Different author
        }

        [Fact]
        public void Updates_bundle()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Original Bundle",
                Price = 1000,
                TourIds = new List<long> { -2, -4 }
            };
            var created = service.Create(createDto, -11);

            var updateDto = new BundleUpdateDto
            {
                Id = created.Id,
                Name = "Updated Bundle",
                Price = 1500,
                TourIds = new List<long> { -2 }
            };

            // Act
            var result = service.Update(updateDto, -11);

            // Assert
            result.Name.ShouldBe("Updated Bundle");
            result.Price.ShouldBe(1500);
            result.TourIds.Count.ShouldBe(1);
        }

        [Fact]
        public void Updates_bundle_fails_not_owner()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Bundle",
                Price = 1000,
                TourIds = new List<long> { -2 }
            };
            var created = service.Create(createDto, -11);

            var updateDto = new BundleUpdateDto
            {
                Id = created.Id,
                Name = "Hacked Bundle",
                Price = 1,
                TourIds = new List<long> { -2 }
            };

            // Act & Assert
            Should.Throw<Exception>(() => service.Update(updateDto, -12)); // Different author
        }

        [Fact]
        public void Deletes_draft_bundle()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Bundle To Delete",
                Price = 1000,
                TourIds = new List<long> { -2 }
            };
            var created = service.Create(createDto, -11);

            // Act
            service.Delete(created.Id, -11);

            // Assert
            Should.Throw<Exception>(() => service.GetById(created.Id));
        }

        [Fact]
        public void Deletes_bundle_fails_not_owner()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Protected Bundle",
                Price = 1000,
                TourIds = new List<long> { -2 }
            };
            var created = service.Create(createDto, -11);

            // Act & Assert
            Should.Throw<Exception>(() => service.Delete(created.Id, -12));
        }

        [Fact]
        public void Gets_bundle_by_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Test Bundle",
                Price = 1200,
                TourIds = new List<long> { -2, -4 }
            };
            var created = service.Create(createDto, -11);

            // Act
            var result = service.GetById(created.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test Bundle");
            result.Tours.Count.ShouldBe(2);
            result.TotalToursPrice.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Gets_bundles_by_author_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            // Act
            var result = service.GetByAuthorId(-11);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void Publishes_bundle_with_two_published_tours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Bundle To Publish",
                Price = 2000,
                TourIds = new List<long> { -2, -4 } // Both are published
            };
            var created = service.Create(createDto, -11);

            // Act
            var result = service.Publish(created.Id, -11);

            // Assert
            result.Status.ShouldBe(1); // Published
            result.PublishedAt.ShouldNotBeNull();
        }

        [Fact]
        public void Publishes_bundle_fails_less_than_two_published_tours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Bundle With Draft Tours",
                Price = 1000,
                TourIds = new List<long> { -1, -2 } // -1 is draft, -2 is published
            };
            var created = service.Create(createDto, -11);

            // Act & Assert
            Should.Throw<Exception>(() => service.Publish(created.Id, -11));
        }

        [Fact]
        public void Publishes_bundle_fails_not_owner()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Bundle",
                Price = 1000,
                TourIds = new List<long> { -2, -4 }
            };
            var created = service.Create(createDto, -11);

            // Act & Assert
            Should.Throw<Exception>(() => service.Publish(created.Id, -12));
        }

        [Fact]
        public void Archives_published_bundle()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Bundle To Archive",
                Price = 1500,
                TourIds = new List<long> { -2, -4 }
            };
            var created = service.Create(createDto, -11);
            service.Publish(created.Id, -11);

            // Act
            var result = service.Archive(created.Id, -11);

            // Assert
            result.Status.ShouldBe(2); // Archived
            result.ArchivedAt.ShouldNotBeNull();
        }

        [Fact]
        public void Archives_bundle_fails_not_published()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Draft Bundle",
                Price = 1000,
                TourIds = new List<long> { -2 }
            };
            var created = service.Create(createDto, -11);

            // Act & Assert
            Should.Throw<Exception>(() => service.Archive(created.Id, -11));
        }

        [Fact]
        public void Deletes_published_bundle_fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Published Bundle",
                Price = 1500,
                TourIds = new List<long> { -2, -4 }
            };
            var created = service.Create(createDto, -11);
            service.Publish(created.Id, -11);

            // Act & Assert
            Should.Throw<Exception>(() => service.Delete(created.Id, -11));
        }

        [Fact]
        public void Updates_archived_bundle_fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Bundle",
                Price = 1500,
                TourIds = new List<long> { -2, -4 }
            };
            var created = service.Create(createDto, -11);
            service.Publish(created.Id, -11);
            service.Archive(created.Id, -11);

            var updateDto = new BundleUpdateDto
            {
                Id = created.Id,
                Name = "Updated Archived",
                Price = 2000,
                TourIds = new List<long> { -2 }
            };

            // Act & Assert
            Should.Throw<Exception>(() => service.Update(updateDto, -11));
        }

        [Fact]
        public void Gets_published_bundles()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var createDto = new BundleCreateDto
            {
                Name = "Public Bundle",
                Price = 1800,
                TourIds = new List<long> { -2, -4 }
            };
            var created = service.Create(createDto, -11);
            service.Publish(created.Id, -11);

            // Act
            var result = service.GetPublished();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.ShouldContain(b => b.Id == created.Id);
        }

        [Fact]
        public void Creates_bundle_with_empty_name_fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var bundleDto = new BundleCreateDto
            {
                Name = "",
                Price = 1000,
                TourIds = new List<long> { -2 }
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => service.Create(bundleDto, -11));
        }

        [Fact]
        public void Creates_bundle_with_negative_price_fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var bundleDto = new BundleCreateDto
            {
                Name = "Invalid Price Bundle",
                Price = -100,
                TourIds = new List<long> { -2 }
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => service.Create(bundleDto, -11));
        }

        [Fact]
        public void Creates_bundle_with_no_tours_fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBundleService>();

            var bundleDto = new BundleCreateDto
            {
                Name = "Empty Bundle",
                Price = 1000,
                TourIds = new List<long>()
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => service.Create(bundleDto, -11));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubCommandTests : BaseStakeholdersIntegrationTest
{
    public ClubCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_club()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var clubDto = new ClubCreateDto
        {
            Name = "New Adventure Club",
            Description = "A club for adventure enthusiasts",
            FeaturedImageUrl = "https://example.com/featured.jpg",
            GalleryImageUrls = new List<string>
            {
                "https://example.com/gallery1.jpg",
                "https://example.com/gallery2.jpg"
            }
        };

        // Act
        var result = service.Create(clubDto, -1);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(clubDto.Name);
        result.Description.ShouldBe(clubDto.Description);
        result.OwnerId.ShouldBe(-1);
        result.FeaturedImageId.ShouldNotBe(0);
        result.FeaturedImage.ShouldNotBeNull();
        result.FeaturedImage.ImageUrl.ShouldBe("https://example.com/featured.jpg");
        result.GalleryImages.Count.ShouldBe(2);
    }

    [Fact]
    public void Updates_club()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var updateDto = new ClubUpdateDto
        {
            Name = "Updated Club Name",
            Description = "Updated description"
        };

        // Act
        var result = service.Update(-1, updateDto, -21);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Club Name");
        result.Description.ShouldBe("Updated description");
    }

    [Fact]
    public void Adds_images_to_gallery()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var updateDto = new ClubUpdateDto
        {
            NewGalleryImageUrls = new List<string>
            {
                "https://example.com/new1.jpg",
                "https://example.com/new2.jpg"
            }
        };

        // Act
        var result = service.Update(-1, updateDto, -21);

        // Assert
        result.ShouldNotBeNull();
        result.GalleryImages.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Removes_image_from_gallery()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var addDto = new ClubUpdateDto
        {
            NewGalleryImageUrls = new List<string> { "https://example.com/to-remove.jpg" }
        };
        var clubWithImage = service.Update(-1, addDto, -21);
        var imageIdToRemove = clubWithImage.GalleryImages[0].Id;

        // Act
        var removeDto = new ClubUpdateDto
        {
            RemovedGalleryImageIds = new List<long> { imageIdToRemove }
        };
        var result = service.Update(-1, removeDto, -21);

        // Assert
        result.GalleryImages.ShouldNotContain(img => img.Id == imageIdToRemove);
    }

    [Fact]
    public void Promotes_gallery_image_to_featured()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var addDto = new ClubUpdateDto
        {
            NewGalleryImageUrls = new List<string> { "https://example.com/new-featured.jpg" }
        };
        var clubWithGallery = service.Update(-2, addDto, -22);
        var galleryImageId = clubWithGallery.GalleryImages[0].Id;

        // Act
        var promoteDto = new ClubUpdateDto
        {
            PromoteGalleryImageId = galleryImageId
        };
        var result = service.Update(-2, promoteDto, -22);

        // Assert
        result.FeaturedImageId.ShouldBe(galleryImageId);
        result.GalleryImages.ShouldNotContain(img => img.Id == galleryImageId);
    }

    [Fact]
    public void Updates_featured_image()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var updateDto = new ClubUpdateDto
        {
            NewFeaturedImageUrl = "https://example.com/new-featured-image.jpg"
        };

        // Act
        var result = service.Update(-2, updateDto, -22);

        // Assert
        result.FeaturedImage.ImageUrl.ShouldBe("https://example.com/new-featured-image.jpg");
    }

    [Fact]
    public void Deletes_club()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        // Act
        service.Delete(-3, -23);

        // Assert
        Should.Throw<KeyNotFoundException>(() => service.Get(-4));
    }

    [Fact]
    public void Fails_to_update_other_users_club()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var updateDto = new ClubUpdateDto
        {
            Name = "Hacked Club Name",
            Description = "Trying to hack"
        };

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() =>
            service.Update(-1, updateDto, -999)
        );
    }

    [Fact]
    public void Fails_to_delete_other_users_club()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() =>
            service.Delete(-1, -999)
        );
    }

    [Fact]
    public void Fails_to_create_club_without_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var clubDto = new ClubCreateDto
        {
            Name = "",
            Description = "Valid description",
            FeaturedImageUrl = "https://example.com/featured.jpg"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.Create(clubDto, -1)
        ).Message.ShouldBe("Club name is required.");
    }

    [Fact]
    public void Fails_to_create_club_without_description()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var clubDto = new ClubCreateDto
        {
            Name = "Valid Name",
            Description = "",
            FeaturedImageUrl = "https://example.com/featured.jpg"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.Create(clubDto, -1)
        ).Message.ShouldBe("Club description is required.");
    }

    [Fact]
    public void Fails_to_create_club_without_featured_image()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        var clubDto = new ClubCreateDto
        {
            Name = "Valid Name",
            Description = "Valid description",
            FeaturedImageUrl = ""
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.Create(clubDto, -1)
        ).Message.ShouldBe("Featured image URL is required.");
    }
}

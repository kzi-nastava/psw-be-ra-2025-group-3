using System;
using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit;

public class KeyPointTests
{
    [Theory]
    [InlineData(1, "Trg Slobode", "Glavni trg", "http://img.jpg", "Tajna", 45.2500, 19.8300)]
    [InlineData(100, "Tvrđava", "Stara tvrđava", "", "Secret", 45.2600, 19.8400)]
    [InlineData(5, "KP", "Opis", "url", "", -90, -180)]
    [InlineData(5, "KP", "Opis", "url", "", 90, 180)]
    public void Creates_valid_key_point(long tourId, string name, string description,
        string imageUrl, string secret, double latitude, double longitude)
    {
        // Act
        var keyPoint = new KeyPoint(tourId, name, description, imageUrl, secret, latitude, longitude);

        // Assert
        keyPoint.ShouldNotBeNull();
        keyPoint.TourId.ShouldBe(tourId);
        keyPoint.Name.ShouldBe(name);
        keyPoint.Description.ShouldBe(description);
        keyPoint.ImageUrl.ShouldBe(imageUrl);
        keyPoint.Secret.ShouldBe(secret);
        keyPoint.Latitude.ShouldBe(latitude);
        keyPoint.Longitude.ShouldBe(longitude);
    }

    [Theory]
    [InlineData(0, "Name", "Desc", "", "", 45.25, 19.83)]           // Invalid tourId
    [InlineData(-1, "Name", "Desc", "", "", 45.25, 19.83)]          // Negative tourId
    [InlineData(1, "", "Desc", "", "", 45.25, 19.83)]               // Empty name
    [InlineData(1, "   ", "Desc", "", "", 45.25, 19.83)]            // Whitespace name
    [InlineData(1, null, "Desc", "", "", 45.25, 19.83)]             // Null name
    [InlineData(1, "Name", "", "", "", 45.25, 19.83)]               // Empty description
    [InlineData(1, "Name", "   ", "", "", 45.25, 19.83)]            // Whitespace description
    [InlineData(1, "Name", null, "", "", 45.25, 19.83)]             // Null description
    [InlineData(1, "Name", "Desc", "", "", -91, 19.83)]             // Latitude too low
    [InlineData(1, "Name", "Desc", "", "", 91, 19.83)]              // Latitude too high
    [InlineData(1, "Name", "Desc", "", "", 45.25, -181)]            // Longitude too low
    [InlineData(1, "Name", "Desc", "", "", 45.25, 181)]             // Longitude too high
    public void Fails_to_create_with_invalid_data(long tourId, string name, string description,
        string imageUrl, string secret, double latitude, double longitude)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new KeyPoint(tourId, name, description, imageUrl, secret, latitude, longitude));
    }

    [Fact]
    public void Updates_key_point_successfully()
    {
        // Arrange
        var keyPoint = new KeyPoint(1, "Staro ime", "Stari opis", "old.jpg", "Stara tajna", 45.25, 19.83);

        // Act
        keyPoint.Update("Novo ime", "Novi opis", "new.jpg", "Nova tajna", 45.26, 19.84);

        // Assert
        keyPoint.Name.ShouldBe("Novo ime");
        keyPoint.Description.ShouldBe("Novi opis");
        keyPoint.ImageUrl.ShouldBe("new.jpg");
        keyPoint.Secret.ShouldBe("Nova tajna");
        keyPoint.Latitude.ShouldBe(45.26);
        keyPoint.Longitude.ShouldBe(19.84);
        keyPoint.TourId.ShouldBe(1); // TourId se ne menja
    }

    [Theory]
    [InlineData("", "Opis", "url", "secret", 45.25, 19.83)]           // Empty name
    [InlineData("   ", "Opis", "url", "secret", 45.25, 19.83)]        // Whitespace name
    [InlineData(null, "Opis", "url", "secret", 45.25, 19.83)]         // Null name
    [InlineData("Ime", "", "url", "secret", 45.25, 19.83)]            // Empty description
    [InlineData("Ime", "   ", "url", "secret", 45.25, 19.83)]         // Whitespace description
    [InlineData("Ime", null, "url", "secret", 45.25, 19.83)]          // Null description
    [InlineData("Ime", "Opis", "url", "secret", -91, 19.83)]          // Invalid latitude low
    [InlineData("Ime", "Opis", "url", "secret", 91, 19.83)]           // Invalid latitude high
    [InlineData("Ime", "Opis", "url", "secret", 45.25, -181)]         // Invalid longitude low
    [InlineData("Ime", "Opis", "url", "secret", 45.25, 181)]          // Invalid longitude high
    public void Fails_to_update_with_invalid_data(string name, string description,
        string imageUrl, string secret, double latitude, double longitude)
    {
        // Arrange
        var keyPoint = new KeyPoint(1, "Original", "Original desc", "url", "secret", 45.25, 19.83);

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            keyPoint.Update(name, description, imageUrl, secret, latitude, longitude));
    }

    [Fact]
    public void Update_allows_empty_imageUrl_and_secret()
    {
        // Arrange
        var keyPoint = new KeyPoint(1, "Ime", "Opis", "old.jpg", "Stara tajna", 45.25, 19.83);

        // Act
        keyPoint.Update("Novo ime", "Novi opis", "", "", 45.26, 19.84);

        // Assert
        keyPoint.ImageUrl.ShouldBe("");
        keyPoint.Secret.ShouldBe("");
    }

    [Fact]
    public void Preserves_tourId_after_update()
    {
        // Arrange
        var originalTourId = 42L;
        var keyPoint = new KeyPoint(originalTourId, "Ime", "Opis", "url", "secret", 45.25, 19.83);

        // Act
        keyPoint.Update("Novo ime", "Novi opis", "new.jpg", "Nova tajna", 45.26, 19.84);

        // Assert
        keyPoint.TourId.ShouldBe(originalTourId);
    }

    [Theory]
    [InlineData(-90, -180)]
    [InlineData(-90, 180)]
    [InlineData(90, -180)]
    [InlineData(90, 180)]
    [InlineData(0, 0)]
    public void Accepts_boundary_coordinates(double latitude, double longitude)
    {
        // Act
        var keyPoint = new KeyPoint(1, "Boundary test", "Testing boundaries", "", "", latitude, longitude);

        // Assert
        keyPoint.Latitude.ShouldBe(latitude);
        keyPoint.Longitude.ShouldBe(longitude);
    }

    [Theory]
    [InlineData(-90.0001, 0)]
    [InlineData(90.0001, 0)]
    [InlineData(0, -180.0001)]
    [InlineData(0, 180.0001)]
    public void Rejects_coordinates_outside_boundaries(double latitude, double longitude)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new KeyPoint(1, "Test", "Test desc", "", "", latitude, longitude));
    }
}
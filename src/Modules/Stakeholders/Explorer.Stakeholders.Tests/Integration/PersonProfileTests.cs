using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Tests;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers; // <-- VAŽNO

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class PersonProfileTests : BaseStakeholdersIntegrationTest
{
    public PersonProfileTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_profile_succeeds()
    {
        // Arrange
        // Kreiramo klijent I ručno se logujemo da dobijemo token
        var client = Factory.CreateClient();
        var token = GetJwtToken("turista1@gmail.com", "turista1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = client.GetAsync("api/stakeholders/person").Result;

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var responseDto = ExtractResult<PersonDto>(response);
        responseDto.ShouldNotBeNull();
        responseDto.Name.ShouldBe("Pera");
        responseDto.Biography.ShouldBe("Pera Peric, turista.");
        responseDto.Quote.ShouldBe("Perin moto.");
    }

    [Fact]
    public void Get_profile_fails_unauthorized()
    {
        // Arrange
        var client = Factory.CreateClient(); // Klijent koji nije ulogovan

        // Act
        var response = client.GetAsync("api/stakeholders/person").Result;

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void Update_profile_succeeds()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = GetJwtToken("turista2@gmail.com", "turista2"); // Logujemo se kao Turista 2
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updatedDto = new PersonDto
        {
            UserId = -22,
            Name = "Mika",
            Surname = "Mikić",
            Email = "mika.novo@email.com",
            Biography = "Moja NOVA biografija.",
            Quote = "Moj NOVI moto."
        };

        // Act
        var response = client.PutAsJsonAsync("api/stakeholders/person", updatedDto).Result;

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var responseDto = ExtractResult<PersonDto>(response);
        responseDto.ShouldNotBeNull();
        responseDto.Email.ShouldBe("mika.novo@email.com");
        responseDto.Biography.ShouldBe("Moja NOVA biografija.");
    }

    [Fact]
    public void Update_profile_fails_invalid_email()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = GetJwtToken("turista2@gmail.com", "turista2");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var invalidDto = new PersonDto
        {
            UserId = -22,
            Name = "Mika",
            Surname = "Mikić",
            Email = "invalid-email" // Nema @
        };

        // Act
        var response = client.PutAsJsonAsync("api/stakeholders/person", invalidDto).Result;

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    // --- Helper Metodi ---

    private static T ExtractResult<T>(HttpResponseMessage response)
    {
        var json = response.Content.ReadAsStringAsync().Result;
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<T>(json, options) ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    private string GetJwtToken(string username, string password)
    {
        var client = Factory.CreateClient();
        var loginDto = new CredentialsDto { Username = username, Password = password };
        var response = client.PostAsJsonAsync("api/users/login", loginDto).Result;
        var authResponse = ExtractResult<AuthenticationTokensDto>(response);
        return authResponse.AccessToken;
    }

    // Dodajemo DTO klase koje su nam potrebne za helper metod
    private class CredentialsDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    private class AuthenticationTokensDto
    {
        public string AccessToken { get; set; }
        public long Id { get; set; }
    }
}
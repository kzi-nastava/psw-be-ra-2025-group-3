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

    // ---------------------
    // CREATE PERSON (ADMIN)
    // ---------------------
    [Fact]
    public void Create_person_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin"); // admin iz tvojih podataka
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new AccountRegistrationDto
        {
            Username = "noviuser",
            Password = "Test123!",
            Role = "Author",
            Name = "Test",
            Surname = "Person",
            Email = "test.person@gmail.com"
        };

        var response = client.PostAsJsonAsync("api/stakeholders/person", dto).Result;

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);

        var person = ExtractResult<PersonDto>(response);
        person.ShouldNotBeNull();
        person.Name.ShouldBe("Test");
        person.Email.ShouldBe("test.person@gmail.com");
    }

    [Fact]
    public void Create_person_fails_invalid_email()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new AccountRegistrationDto
        {
            Username = "losmail",
            Password = "Test123!",
            Role = "Author",
            Name = "Bad",
            Surname = "Email",
            Email = "invalid-email" // nevalidan email
        };

        var response = client.PostAsJsonAsync("api/stakeholders/person", dto).Result;

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableEntity);
    }

    // ---------------------
    // GET ALL PEOPLE
    // ---------------------
    [Fact]
    public void Get_all_people_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = client.GetAsync("api/stakeholders/person/all").Result;
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var people = ExtractResult<List<PersonDto>>(response);
        people.ShouldNotBeNull();
        people.Count.ShouldBeGreaterThan(0);
        people.Any(p => p.UserId == -1).ShouldBeTrue(); // admin
        people.Any(p => p.UserId == -21).ShouldBeTrue(); // Pera
        people.Any(p => p.UserId == -22).ShouldBeTrue(); // Mika
        people.Any(p => p.UserId == -23).ShouldBeTrue(); // Steva
    }

    // ---------------------
    // GET PERSON BY ID
    // ---------------------
    [Fact]
    public void Get_person_by_id_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = client.GetAsync("api/stakeholders/person/-21").Result; // Pera
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var person = ExtractResult<PersonDto>(response);
        person.ShouldNotBeNull();
        person.UserId.ShouldBe(-21);
        person.Name.ShouldBe("Pera");
        person.Email.ShouldBe("turista1@gmail.com");
    }

    // ---------------------
    // BLOCK PERSON
    // ---------------------
    [Fact]
    public void Block_person_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = client.PutAsync("api/stakeholders/person/-22/block", null).Result; // Mika
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

        var getResponse = client.GetAsync("api/stakeholders/person/-22").Result;
        var person = ExtractResult<PersonDto>(getResponse);
        person.IsActive.ShouldBeFalse();
    }

    // ---------------------
    // UNBLOCK PERSON
    // ---------------------
    [Fact]
    public void Unblock_person_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = client.PutAsync("api/stakeholders/person/-22/unblock", null).Result; // Mika
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

        var getResponse = client.GetAsync("api/stakeholders/person/-22").Result;
        var person = ExtractResult<PersonDto>(getResponse);
        person.IsActive.ShouldBeTrue();
    }

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
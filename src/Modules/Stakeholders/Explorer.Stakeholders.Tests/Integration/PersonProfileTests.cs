using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Tests;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class PersonProfileTests : BaseStakeholdersIntegrationTest
{
    public PersonProfileTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_profile_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("turista1@gmail.com", "turista1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = client.GetAsync("api/stakeholders/person").Result;

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
        var client = Factory.CreateClient();

        var response = client.GetAsync("api/stakeholders/person").Result;

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void Update_profile_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("turista2@gmail.com", "turista2");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updatedDto = new PersonDto
        {
            UserId = -22, // biće ignorisano u kontroleru, koristi se ID iz tokena
            Name = "Mika",
            Surname = "Mikić",
            Email = "mika.novo@email.com",
            Biography = "Moja NOVA biografija.",
            Quote = "Moj NOVI moto."
        };

        var response = client.PutAsJsonAsync("api/stakeholders/person", updatedDto).Result;

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var responseDto = ExtractResult<PersonDto>(response);
        responseDto.ShouldNotBeNull();
        responseDto.Email.ShouldBe("mika.novo@email.com");
        responseDto.Biography.ShouldBe("Moja NOVA biografija.");
    }

    [Fact]
    public void Update_profile_fails_invalid_email()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("turista2@gmail.com", "turista2");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var invalidDto = new PersonDto
        {
            UserId = -22,
            Name = "Mika",
            Surname = "Mikić",
            Email = "invalid-email"
        };

        var response = client.PutAsJsonAsync("api/stakeholders/person", invalidDto).Result;

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    // ---------------------
    // CREATE PERSON (ADMIN)
    // ---------------------
    [Fact]
    public void Create_person_succeeds()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new AccountRegistrationDto
        {
            Username = "noviuser",
            Password = "Test123!",
            Role = "Author",
            Name = "Test",
            Surname = "Person",
            Email = "test.person@gmail.com"
            // NEMA Biography / Url / Quote – uklonila si ih iz AccountRegistrationDto
        };

        var response = client.PostAsJsonAsync("api/stakeholders/person", dto).Result;

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);

        var person = ExtractResult<PersonDto>(response);
        person.ShouldNotBeNull();
        person.UserId.ShouldNotBe(0);
        person.Name.ShouldBe("Test");
        person.Surname.ShouldBe("Person");
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

        // Ovde se oslanjamo na validaciju iz servisa / domena – u šablonu obično ide 422
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableEntity);
    }

    // ---------------------
    // GET ALL PEOPLE
    // ---------------------
    [Fact]
    public void Get_all_people_excludes_logged_in_admin_and_returns_others()
    {
        var client = Factory.CreateClient();
        var token = GetJwtToken("admin@gmail.com", "admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = client.GetAsync("api/stakeholders/person/all").Result;
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var people = ExtractResult<List<PersonDto>>(response);
        people.ShouldNotBeNull();
        people.Count.ShouldBeGreaterThan(0);

        // NOVA LOGIKA: servis vraća sve osobe OSIM trenutno ulogovane (admin je -1)
        people.Any(p => p.UserId == -1).ShouldBeFalse();   // admin NE SME da bude u listi

        // Ostali seed-ovani korisnici treba i dalje da budu u listi
        people.Any(p => p.UserId == -21).ShouldBeTrue();   // Pera
        people.Any(p => p.UserId == -22).ShouldBeTrue();   // Mika
        people.Any(p => p.UserId == -23).ShouldBeTrue();   // Steva
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

    // ---------------------
    // HELPER METODI
    // ---------------------
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

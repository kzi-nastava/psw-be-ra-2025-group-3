using Explorer.BuildingBlocks.Core.Domain;
using System.Net.Mail;

namespace Explorer.Stakeholders.Core.Domain;

public enum PersonRole
{
    Administrator,
    Author,
    Tourist
}


public class Person : Entity
{

    public long UserId { get; init; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? Biography { get; private set; }
    public string? Quote { get; private set; }

    public Person(
    long userId,
    string name,
    string surname,
    string email,
    string? profilePictureUrl = null,
    string? biography = null,
    string? quote = null)
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        Email = email;     // Ako nije prosleđeno, prazno
        ProfilePictureUrl = profilePictureUrl;
        Biography = biography;
        Quote = quote;

        Validate();
    }



    public void Update(string name, string surname, string email,
                       string? profilePictureUrl, string? biography, string? quote)
    {
        Name = name;
        Surname = surname;
        Email = email;
        ProfilePictureUrl = profilePictureUrl;
        Biography = biography;
        Quote = quote;
        Validate();
    }


  private void Validate()
     {
         if (UserId == 0) throw new ArgumentException("Invalid UserId");
         if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
         if (string.IsNullOrWhiteSpace(Surname)) throw new ArgumentException("Invalid Surname");
         if (!MailAddress.TryCreate(Email, out _)) throw new ArgumentException("Invalid Email");
     } 
    

}
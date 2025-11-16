namespace Explorer.Stakeholders.API.Dtos;

public class PersonDto
{
    public long UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Biography { get; set; }
    public string? Quote { get; set; }
}
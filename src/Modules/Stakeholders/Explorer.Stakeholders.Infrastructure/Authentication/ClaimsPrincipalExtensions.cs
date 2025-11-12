using System.Security.Claims;

namespace Explorer.Stakeholders.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static int PersonId(this ClaimsPrincipal user)
        => int.Parse(user.Claims.First(i => i.Type == "personId").Value);
}
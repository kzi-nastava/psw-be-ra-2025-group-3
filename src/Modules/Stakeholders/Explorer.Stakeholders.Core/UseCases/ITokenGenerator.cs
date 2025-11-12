using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.UseCases;

public interface ITokenGenerator
{
    AuthenticationTokensDto GenerateAccessToken(User user, long personId);
}
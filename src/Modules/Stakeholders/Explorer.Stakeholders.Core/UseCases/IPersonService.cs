using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.Core.UseCases;

public interface IPersonService
{
    PersonDto Get(long personId);
    PersonDto Update(PersonDto personDto);

    PersonDto Create(AccountRegistrationDto dto);
    List<PersonDto> GetAll();

    void Block(long id);
    void Unblock(long id);
}
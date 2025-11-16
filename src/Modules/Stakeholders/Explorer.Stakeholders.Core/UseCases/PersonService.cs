using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    public PersonService(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public PersonDto Get(long personId)
    {
        var person = _personRepository.Get(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with ID {personId} not found.");
        return _mapper.Map<PersonDto>(person);
    }

    public PersonDto Update(PersonDto personDto)
    {
        var personId = personDto.UserId;
        var person = _personRepository.Get(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with ID {personId} not found.");

        person.Update(
            personDto.Name,
            personDto.Surname,
            personDto.Email,
            personDto.ProfilePictureUrl,
            personDto.Biography,
            personDto.Quote
        );

        var updatedPerson = _personRepository.Update(person);
        return _mapper.Map<PersonDto>(updatedPerson);
    }
}
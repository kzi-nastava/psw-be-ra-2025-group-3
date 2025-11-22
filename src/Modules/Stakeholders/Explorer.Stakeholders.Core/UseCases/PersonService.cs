using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IUserRepository _userRepository;   // DODATO
    private readonly IMapper _mapper;

    public PersonService(IPersonRepository personRepository, IUserRepository userRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public PersonDto Get(long personId)
    {
        var person = _personRepository.Get(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with ID {personId} not found.");

        var dto = _mapper.Map<PersonDto>(person);

        // mapiramo IsActive iz User entiteta
        var user = _userRepository.Get(person.UserId);
        dto.IsActive = user?.IsActive ?? false;

        return dto;
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

    public PersonDto Create(AccountRegistrationDto dto)
    {
        // --- VALIDACIJE (ostavljamo ih iste) ---


        if (_personRepository.EmailExists(dto.Email))
            throw new EntityValidationException("Email already exists.");

        if (!dto.Email.Contains("@"))
            throw new EntityValidationException("Email must contain '@'.");

        if (!dto.Email.EndsWith("@gmail.com"))
            throw new EntityValidationException("Email must be a valid @gmail.com address.");

        // Administrator ili Autor se kreiraju ručno u admin panelu,
        // pa po defaultu dodeljujemo ispravnu User rolu:
        var role = Enum.Parse<UserRole>(dto.Role, true);

        // ili UserRole.Author — zavisi od toga šta želiš da kreiraš

        // --- KREIRANJE USERA ---

        if (_userRepository.Exists(dto.Username))
            throw new EntityValidationException("Username already exists in users.");

        var user = new User(
            dto.Username,
            dto.Password,
            role,
            true // active
        );

        user = _userRepository.Create(user);

        // --- KREIRANJE PERSON-A ---

        var person = new Person(
            user.Id,
            dto.Name,
            dto.Surname,
            dto.Email
        );

        person = _personRepository.Create(person);

        return _mapper.Map<PersonDto>(person);
    }


    public List<PersonDto> GetAll()
    {
        var people = _personRepository.GetAll();
        return people.Select(_mapper.Map<PersonDto>).ToList();
    }

    public void Block(long id)
    {
        var user = _userRepository.Get(id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (user.Role == UserRole.Administrator)
            throw new EntityValidationException("Cannot block administrator accounts.");

        user.Block();
        _userRepository.Update(user);
    }

    public void Unblock(long id)
    {
        var user = _userRepository.Get(id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Unblock();
        _userRepository.Update(user);
    }
}
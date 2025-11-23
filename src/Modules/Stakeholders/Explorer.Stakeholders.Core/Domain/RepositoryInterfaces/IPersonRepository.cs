using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPersonRepository
{
    Person Create(Person person);
    Person? Get(long id);

    List<Person> GetAll();
    Person Update(Person person);

    bool EmailExists(string email);
}
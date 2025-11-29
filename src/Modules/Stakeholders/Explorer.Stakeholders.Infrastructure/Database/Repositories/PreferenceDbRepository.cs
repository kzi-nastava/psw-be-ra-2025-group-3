using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PreferenceDbRepository : IPreferenceRepository
{
    private readonly StakeholdersContext _context;

    public PreferenceDbRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public Preference Create(Preference preference)
    {
        _context.Preferences.Add(preference);
        _context.SaveChanges();
        return preference;
    }

    public Preference Update(Preference preference)
    {
        _context.Preferences.Update(preference);
        _context.SaveChanges();
        return preference;
    }

    public void Delete(long touristId)
    {
        var preference = GetByTouristId(touristId);
        if (preference != null)
        {
            _context.Preferences.Remove(preference);
            _context.SaveChanges();
        }
    }

    public Preference? GetByTouristId(long touristId)
    {
        return _context.Preferences
            .FirstOrDefault(p => p.TouristId == touristId);
    }
}

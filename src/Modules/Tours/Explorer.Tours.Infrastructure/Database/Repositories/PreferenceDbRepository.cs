using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class PreferenceDbRepository : IPreferenceRepository
{
    private readonly ToursContext _context;

    public PreferenceDbRepository(ToursContext context)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class Facility : Entity
{
    public string Name { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public FacilityCategory Category { get; private set; }

    public Facility() { }

    public Facility(string name, double latitude, double longitude, FacilityCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
    }

    public void Update(string name, double latitude, double longitude, FacilityCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
    }
}
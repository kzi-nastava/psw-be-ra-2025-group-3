using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Diary : AggregateRoot
    {
        public string Title { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DiaryStatus Status { get; private set; }

        public string Country { get; private set; }
        public string? City { get; private set; }

        public int TouristId { get; private set; }

        private Diary() { } // EF

        public Diary(string title, string country, string? city, int touristId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.");

            Title = title.Trim();
            Country = country.Trim();
            City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();

            TouristId = touristId;

            CreatedAt = DateTime.UtcNow;
            Status = DiaryStatus.Draft;
        }

        public void Update(string title, string country, string? city, int userId)
        {
            EnsureOwner(userId);

            if (Status == DiaryStatus.Archived)
                throw new InvalidOperationException("Cannot modify an archived diary.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.");

            Title = title.Trim();
            Country = country.Trim();
            City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        }

        public void Archive(int userId)
        {
            EnsureOwner(userId);
            Status = DiaryStatus.Archived;
        }

        public void EnsureOwner(int userId)
        {
            if (userId != TouristId)
                throw new UnauthorizedAccessException("You are not the owner of this diary.");
        }
    }
}

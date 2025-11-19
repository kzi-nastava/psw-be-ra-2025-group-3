using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain; 

namespace Explorer.Tours.Core.Domain
{
    public class AwardEvent : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Year { get; private set; }
        public AwardEventStatus Status { get; private set; }
        public DateTime VotingStartDate { get; private set; }
        public DateTime VotingEndDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private AwardEvent() { }

        public AwardEvent(string name, string description, int year, DateTime votingStartDate, DateTime votingEndDate)
        {
            // Validacija
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (year <= 2000) throw new ArgumentException("Invalid year.");
            if (votingStartDate >= votingEndDate) throw new ArgumentException("Voting start date must be before end date.");

            Name = name;
            Description = description;
            Year = year;
            VotingStartDate = votingStartDate;
            VotingEndDate = votingEndDate;

            Status = AwardEventStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description, int year, DateTime votingStartDate, DateTime votingEndDate)
        {
            // Validacija
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (year <= 2000) throw new ArgumentException("Invalid year.");
            if (votingStartDate >= votingEndDate) throw new ArgumentException("Voting start date must be before end date.");

            Name = name;
            Description = description;
            Year = year;
            VotingStartDate = votingStartDate;
            VotingEndDate = votingEndDate;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

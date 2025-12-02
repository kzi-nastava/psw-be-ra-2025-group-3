using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Tour : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public TourDifficulty Difficulty { get; private set; }
    public TourStatus Status { get; private set; }
    public decimal Price { get; private set; }
    public long AuthorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public List<string> Tags { get; private set; } //Lista stringova

    // Lista opreme potrebne za turu
    public ICollection<Equipment> Equipment { get; private set; }

    // Prazan konstruktor za Entity Framework
    public Tour() { }

    // Konstruktor za kreiranje nove ture
    public Tour(string name, string description, TourDifficulty difficulty, long authorId, List<string>? tags = null)
    {
        //Error handeling
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tour name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Tour description cannot be empty.", nameof(description));

        Name = name;
        Description = description;
        Difficulty = difficulty;
        Status = TourStatus.Draft; // Default Draft
        Price = 0; // Default 0
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
        Tags = tags ?? new List<string>(); // Inicijalizacija prazne liste ako nije prosleđena
        Equipment = new List<Equipment>(); // Inicijalizacija liste opreme
    }

    // Metoda za izmenu ture
    public void Update(string name, string description, TourDifficulty difficulty, decimal price, List<string>? tags = null)
    {
        // Provera da li je tura arhivirana pre izmene
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Cannot modify an archived tour.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tour name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Tour description cannot be empty.", nameof(description));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        Name = name;
        Description = description;
        Difficulty = difficulty;
        Price = price;
        Tags = tags ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
    }

    // Metoda za objavljivanje ture
    public void Publish()
    {
        if (Status == TourStatus.Published)
            throw new InvalidOperationException("Tour is already published.");

        // Ne moze se objaviti ako je arhivirana
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Cannot publish an archived tour.");

        Status = TourStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    // Metoda za arhiviranje ture
    public void Archive()
    {
        if (Status != TourStatus.Published)
            throw new InvalidOperationException("Only published tours can be archived.");

        Status = TourStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    // Metoda za dodavanje opreme ukoliko tura nije arhivirana
    public void AddEquipment(Equipment equipment)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Cannot add equipment to an archived tour.");

        if (!Equipment.Contains(equipment))
        {
            Equipment.Add(equipment);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    // Metoda za uklanjanje opreme ukoliko tura nije arhivirana
    public void RemoveEquipment(Equipment equipment)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Cannot remove equipment from an archived tour.");

        if (Equipment.Contains(equipment))
        {
            Equipment.Remove(equipment);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
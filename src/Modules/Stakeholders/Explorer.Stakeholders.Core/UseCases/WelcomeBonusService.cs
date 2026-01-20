using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class WelcomeBonusService : IWelcomeBonusService, IInternalWelcomeBonusService
{
    private readonly IWelcomeBonusRepository _bonusRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly Random _random;

    public WelcomeBonusService(IWelcomeBonusRepository bonusRepository, IWalletRepository walletRepository)
    {
        _bonusRepository = bonusRepository;
        _walletRepository = walletRepository;
        _random = new Random();
    }

    public WelcomeBonusDto GetWelcomeBonus(long personId)
    {
        var bonus = _bonusRepository.GetByPersonId(personId);
        if (bonus == null)
            throw new KeyNotFoundException("Welcome bonus not found.");

        return MapToDto(bonus);
    }

    public WelcomeBonusDto CreateWelcomeBonus(long personId)
    {
        // Proveri da li već postoji bonus
        if (_bonusRepository.ExistsForPerson(personId))
        {
            return GetWelcomeBonus(personId);
        }

        // Nasumično dodeli bonus prema verovatnoćama
        var bonusType = GenerateRandomBonusType();
        var bonus = new WelcomeBonus(personId, bonusType);
        var createdBonus = _bonusRepository.Create(bonus);

        // Ako je AC bonus, dodaj u wallet
        if (bonusType == BonusType.AC100 || bonusType == BonusType.AC250 || bonusType == BonusType.AC500)
        {
            var wallet = _walletRepository.GetByPersonId(personId);
            if (wallet != null)
            {
                wallet.AddAc(createdBonus.Value);
                _walletRepository.Update(wallet);
            }
        }

        return MapToDto(createdBonus);
    }

    public WelcomeBonusDto? GetActiveDiscountBonus(long personId)
    {
        var bonus = _bonusRepository.GetByPersonId(personId);
        if (bonus == null || bonus.IsUsed || bonus.IsExpired())
            return null;

        // Vrati samo ako je popust bonus
        if (bonus.BonusType == BonusType.Discount10 || 
            bonus.BonusType == BonusType.Discount20 || 
            bonus.BonusType == BonusType.Discount30)
        {
            return MapToDto(bonus);
        }

        return null;
    }

    public void MarkBonusAsUsed(long personId)
    {
        var bonus = _bonusRepository.GetByPersonId(personId);
        if (bonus == null)
            throw new KeyNotFoundException("Welcome bonus not found.");

        bonus.MarkAsUsed();
        _bonusRepository.Update(bonus);
    }

    private BonusType GenerateRandomBonusType()
    {
        // Verovatnoće:
        // 100 AC: 30% (0-29)
        // 250 AC: 20% (30-49)
        // 500 AC: 10% (50-59)
        // 10% popust: 20% (60-79)
        // 20% popust: 15% (80-94)
        // 30% popust: 5% (95-99)

        var randomValue = _random.Next(0, 100);

        return randomValue switch
        {
            >= 0 and < 30 => BonusType.AC100,
            >= 30 and < 50 => BonusType.AC250,
            >= 50 and < 60 => BonusType.AC500,
            >= 60 and < 80 => BonusType.Discount10,
            >= 80 and < 95 => BonusType.Discount20,
            >= 95 and <= 99 => BonusType.Discount30,
            _ => BonusType.AC100 // fallback
        };
    }

    private WelcomeBonusDto MapToDto(WelcomeBonus bonus)
    {
        return new WelcomeBonusDto
        {
            Id = bonus.Id,
            PersonId = bonus.PersonId,
            BonusType = (BonusTypeDto)(int)bonus.BonusType,
            Value = bonus.Value,
            IsUsed = bonus.IsUsed,
            CreatedAt = bonus.CreatedAt,
            ExpiresAt = bonus.ExpiresAt,
            UsedAt = bonus.UsedAt
        };
    }
}

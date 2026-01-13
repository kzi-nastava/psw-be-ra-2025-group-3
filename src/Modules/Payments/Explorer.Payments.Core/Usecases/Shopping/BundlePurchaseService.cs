using System;
using System.Collections.Generic;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Internal;

namespace Explorer.Payments.Core.UseCases.Shopping
{
    public class BundlePurchaseService : IBundlePurchaseService
    {
        private readonly IBundlePurchaseRecordRepository _bundlePurchaseRecordRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IInternalWalletService _walletService;
        private readonly IInternalNotificationService _notificationService;
        private readonly IInternalBundleService _bundleService;
        private readonly IMapper _mapper;

        public BundlePurchaseService(
            IBundlePurchaseRecordRepository bundlePurchaseRecordRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IInternalWalletService walletService,
            IInternalNotificationService notificationService,
            IInternalBundleService bundleService,
            IMapper mapper)
        {
            _bundlePurchaseRecordRepository = bundlePurchaseRecordRepository;
            _tokenRepository = tokenRepository;
            _walletService = walletService;
            _notificationService = notificationService;
            _bundleService = bundleService;
            _mapper = mapper;
        }

        public BundlePurchaseResultDto PurchaseBundle(long touristId, long bundleId)
        {
            Console.WriteLine($"\n========== BUNDLE PURCHASE START (Tourist {touristId}, Bundle {bundleId}) ==========");

            try
            {
                // 1. Preuzmi bundle
                Console.WriteLine("[1/7] Getting bundle...");
                var bundle = _bundleService.GetById(bundleId);
                if (bundle == null)
                {
                    Console.WriteLine("[ERROR] Bundle not found");
                    return new BundlePurchaseResultDto
                    {
                        Success = false,
                        Message = "Bundle not found.",
                        PurchaseRecord = null,
                        Tokens = new List<TourPurchaseTokenDto>()
                    };
                }

                if (bundle.Status != 1) // Published
                {
                    Console.WriteLine("[ERROR] Bundle not published");
                    return new BundlePurchaseResultDto
                    {
                        Success = false,
                        Message = "Bundle is not available for purchase.",
                        PurchaseRecord = null,
                        Tokens = new List<TourPurchaseTokenDto>()
                    };
                }
                Console.WriteLine($"[SUCCESS] Bundle found: {bundle.Name}, Price: {bundle.Price} AC, Tours: {bundle.TourIds.Count}");

                // 2. Proveri wallet
                Console.WriteLine("[2/7] Getting wallet...");
                var wallet = _walletService.GetWallet(touristId);
                Console.WriteLine($"[SUCCESS] Wallet balance: {wallet.BalanceAc} AC");

                if (wallet.BalanceAc < bundle.Price)
                {
                    Console.WriteLine($"[ERROR] Insufficient funds");
                    return new BundlePurchaseResultDto
                    {
                        Success = false,
                        Message = $"Insufficient Adventure Coins. You need {(bundle.Price - wallet.BalanceAc)} more AC.",
                        PurchaseRecord = null,
                        Tokens = new List<TourPurchaseTokenDto>()
                    };
                }

                // 3. Dedukcija AC
                Console.WriteLine("[3/7] Deducting AC...");
                _walletService.DeductAc(touristId, bundle.Price);
                Console.WriteLine($"[SUCCESS] Deducted {bundle.Price} AC");

                // 4. Kreiranje payment record-a za bundle
                Console.WriteLine("[4/7] Creating bundle purchase record...");
                var purchaseRecord = new BundlePurchaseRecord(touristId, bundleId, bundle.Price);
                var createdRecord = _bundlePurchaseRecordRepository.Create(purchaseRecord);
                Console.WriteLine($"[SUCCESS] Record created: ID={createdRecord.Id}");

                // 5. Kreiranje tokena za svaku turu
                Console.WriteLine("[5/7] Creating tour tokens...");
                var tokenDtos = new List<TourPurchaseTokenDto>();

                foreach (var tourId in bundle.TourIds)
                {
                    Console.WriteLine($"  Creating token for tour {tourId}...");
                    var token = new TourPurchaseToken(touristId, tourId);
                    var createdToken = _tokenRepository.Create(token);
                    Console.WriteLine($"  Token created: ID={createdToken.Id}");

                    tokenDtos.Add(new TourPurchaseTokenDto
                    {
                        Id = createdToken.Id,
                        TouristId = createdToken.TouristId,
                        TourId = createdToken.TourId,
                        Token = createdToken.Token,
                        CreatedAt = createdToken.CreatedAt
                    });
                }
                Console.WriteLine($"[SUCCESS] Created {tokenDtos.Count} tokens");

                // 6. Slanje notifikacije
                Console.WriteLine("[6/7] Sending notification...");
                try
                {
                    _notificationService.CreateBundlePurchaseNotification(touristId, bundleId, bundle.Name);
                    Console.WriteLine("[SUCCESS] Notification sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Notification failed (non-critical): {ex.Message}");
                }

                // 7. Kreiranje rezultata
                Console.WriteLine("[7/7] Creating result DTO...");
                var result = new BundlePurchaseResultDto
                {
                    Success = true,
                    Message = $"Successfully purchased bundle '{bundle.Name}' with {tokenDtos.Count} tour(s)!",
                    PurchaseRecord = new BundlePurchaseRecordDto
                    {
                        Id = createdRecord.Id,
                        TouristId = createdRecord.TouristId,
                        BundleId = createdRecord.BundleId,
                        PriceAc = createdRecord.PriceAc,
                        PurchasedAt = createdRecord.PurchasedAt
                    },
                    Tokens = tokenDtos
                };

                Console.WriteLine("========== BUNDLE PURCHASE COMPLETED SUCCESSFULLY ==========\n");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n========== BUNDLE PURCHASE FAILED ==========");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
                Console.WriteLine("============================================\n");
                throw;
            }

        }
        public List<long> GetPurchasedBundleIds(long touristId)
        {
            return _bundlePurchaseRecordRepository.GetPurchasedBundleIdsByTourist(touristId);
        }
    }
}
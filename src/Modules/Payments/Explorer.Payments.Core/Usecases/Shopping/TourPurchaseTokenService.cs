// src/Modules/Payments/Explorer.Payments.Core/UseCases/Shopping/TourPurchaseTokenService.cs
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TourPurchaseTokenService : ITourPurchaseTokenService, IInternalTokenService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly ITourPurchaseRecordRepository _recordRepository;
        private readonly IInternalWalletService _walletService;
        private readonly IInternalNotificationService _notificationService;
        private readonly IInternalTourService _tourService;
        private readonly IBundlePurchaseRecordRepository _bundlePurchaseRecordRepository;
        private readonly IInternalBundleService _bundleService;
        private readonly IMapper _mapper;

        public TourPurchaseTokenService(
            IShoppingCartRepository cartRepository,
            ITourPurchaseTokenRepository tokenRepository,
            ITourPurchaseRecordRepository recordRepository,
            IBundlePurchaseRecordRepository bundlePurchaseRecordRepository,
            IInternalWalletService walletService,
            IInternalNotificationService notificationService,
            IInternalTourService tourService,
            IInternalBundleService bundleService,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tokenRepository = tokenRepository;
            _recordRepository = recordRepository;
            _bundlePurchaseRecordRepository = bundlePurchaseRecordRepository;
            _walletService = walletService;
            _notificationService = notificationService;
            _tourService = tourService;
            _bundleService = bundleService;
            _mapper = mapper;
        }

        public CheckoutResultDto Checkout(long touristId)
        {
            Console.WriteLine($"\n========== CHECKOUT START (Tourist {touristId}) ==========");

            try
            {
                Console.WriteLine("[1/8] Getting cart...");
                var cart = _cartRepository.GetActiveForTourist(touristId);

                if (cart == null || (cart.Items.Count == 0 && cart.BundleItems.Count == 0))
                {
                    Console.WriteLine("[ERROR] Cart is null or empty");
                    return new CheckoutResultDto
                    {
                        Success = false,
                        Message = "Your cart is empty.",
                        Tokens = new List<TourPurchaseTokenDto>(),
                        PurchaseRecords = new List<TourPurchaseRecordDto>(),
                        BundlePurchaseRecords = new List<BundlePurchaseRecordDto>()
                    };
                }
                Console.WriteLine($"[SUCCESS] Cart has {cart.Items.Count} tour items and {cart.BundleItems.Count} bundle items, Total: {cart.TotalPrice} AC");

                Console.WriteLine("[2/8] Getting wallet...");
                var wallet = _walletService.GetWallet(touristId);
                Console.WriteLine($"[SUCCESS] Wallet balance: {wallet.BalanceAc} AC");

                if (wallet.BalanceAc < cart.TotalPrice)
                {
                    Console.WriteLine($"[ERROR] Insufficient funds");
                    return new CheckoutResultDto
                    {
                        Success = false,
                        Message = $"Insufficient Adventure Coins. You need {(cart.TotalPrice - wallet.BalanceAc)} more AC.",
                        Tokens = new List<TourPurchaseTokenDto>(),
                        PurchaseRecords = new List<TourPurchaseRecordDto>(),
                        BundlePurchaseRecords = new List<BundlePurchaseRecordDto>()
                    };
                }

                Console.WriteLine("[3/8] Deducting AC...");
                _walletService.DeductAc(touristId, cart.TotalPrice);
                Console.WriteLine($"[SUCCESS] Deducted {cart.TotalPrice} AC");

                Console.WriteLine("[4/8] Creating tokens and records...");
                var tokenDtos = new List<TourPurchaseTokenDto>();
                var recordDtos = new List<TourPurchaseRecordDto>();
                var bundleRecordDtos = new List<BundlePurchaseRecordDto>(); // ✅ NOVA LISTA

                // Process individual tours
                foreach (var item in cart.Items)
                {
                    Console.WriteLine($"  Processing tour {item.TourId}...");

                    Console.WriteLine("    Creating token...");
                    var token = new TourPurchaseToken(touristId, item.TourId);
                    var createdToken = _tokenRepository.Create(token);
                    Console.WriteLine($"    Token created: ID={createdToken.Id}");

                    tokenDtos.Add(new TourPurchaseTokenDto
                    {
                        Id = createdToken.Id,
                        TouristId = createdToken.TouristId,
                        TourId = createdToken.TourId,
                        Token = createdToken.Token,
                        CreatedAt = createdToken.CreatedAt
                    });

                    Console.WriteLine("    Creating record...");
                    var record = new TourPurchaseRecord(touristId, item.TourId, item.Price);
                    var createdRecord = _recordRepository.Create(record);
                    Console.WriteLine($"    Record created: ID={createdRecord.Id}");

                    recordDtos.Add(new TourPurchaseRecordDto
                    {
                        Id = createdRecord.Id,
                        TouristId = createdRecord.TouristId,
                        TourId = createdRecord.TourId,
                        PriceAc = createdRecord.PriceAc,
                        PurchasedAt = createdRecord.PurchasedAt
                    });

                    Console.WriteLine("    Sending notification...");
                    try
                    {
                        var tour = _tourService.GetById(item.TourId);
                        if (tour != null)
                        {
                            _notificationService.CreateTourPurchaseNotification(touristId, item.TourId, tour.Name);
                            Console.WriteLine("    Notification sent");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"    Notification failed (non-critical): {ex.Message}");
                    }
                }

                // ✅ Process bundle items
                Console.WriteLine("[4B/8] Processing bundle items...");
                foreach (var bundleItem in cart.BundleItems)
                {
                    Console.WriteLine($"  Processing bundle {bundleItem.BundleId}...");

                    var bundle = _bundleService.GetById(bundleItem.BundleId);
                    if (bundle == null)
                    {
                        Console.WriteLine($"    [WARNING] Bundle {bundleItem.BundleId} not found, skipping");
                        continue;
                    }

                    // ✅ Create bundle purchase record
                    Console.WriteLine("    Creating bundle record...");
                    var bundleRecord = new BundlePurchaseRecord(touristId, bundleItem.BundleId, bundleItem.Price);
                    var createdBundleRecord = _bundlePurchaseRecordRepository.Create(bundleRecord);
                    Console.WriteLine($"    Bundle record created: ID={createdBundleRecord.Id}");

                    // ✅ Add to bundleRecordDtos list
                    bundleRecordDtos.Add(new BundlePurchaseRecordDto
                    {
                        Id = createdBundleRecord.Id,
                        TouristId = createdBundleRecord.TouristId,
                        BundleId = createdBundleRecord.BundleId,
                        PriceAc = createdBundleRecord.PriceAc,
                        PurchasedAt = createdBundleRecord.PurchasedAt
                    });

                    // ✅ Create tokens for all tours in the bundle
                    Console.WriteLine($"    Creating tokens for {bundle.TourIds.Count} tours in bundle...");
                    foreach (var tourId in bundle.TourIds)
                    {
                        Console.WriteLine($"      Creating token for tour {tourId}...");
                        var token = new TourPurchaseToken(touristId, tourId);
                        var createdToken = _tokenRepository.Create(token);

                        tokenDtos.Add(new TourPurchaseTokenDto
                        {
                            Id = createdToken.Id,
                            TouristId = createdToken.TouristId,
                            TourId = createdToken.TourId,
                            Token = createdToken.Token,
                            CreatedAt = createdToken.CreatedAt
                        });
                        Console.WriteLine($"      Token created: ID={createdToken.Id}");
                    }

                    // ✅ Send bundle notification
                    Console.WriteLine("    Sending bundle notification...");
                    try
                    {
                        _notificationService.CreateBundlePurchaseNotification(touristId, bundleItem.BundleId, bundleItem.BundleName);
                        Console.WriteLine("    Bundle notification sent");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"    Bundle notification failed (non-critical): {ex.Message}");
                    }
                }

                Console.WriteLine($"[5/8] Created {tokenDtos.Count} tokens, {recordDtos.Count} tour records, and {bundleRecordDtos.Count} bundle records");

                Console.WriteLine("[6/8] Clearing cart...");
                cart.Clear();
                Console.WriteLine($"[SUCCESS] Cart cleared, Items: {cart.Items.Count}, BundleItems: {cart.BundleItems.Count}");

                Console.WriteLine("[7/8] Updating cart in database...");
                _cartRepository.Update(cart);
                Console.WriteLine("[SUCCESS] Cart updated in database");

                Console.WriteLine("[8/8] Creating result DTO...");
                var totalItems = recordDtos.Count + bundleRecordDtos.Count;
                var result = new CheckoutResultDto
                {
                    Success = true,
                    Message = $"Successfully purchased {recordDtos.Count} tour(s) and {bundleRecordDtos.Count} bundle(s)!",
                    Tokens = tokenDtos,
                    PurchaseRecords = recordDtos,
                    BundlePurchaseRecords = bundleRecordDtos // ✅ DODAJ bundle records
                };
                Console.WriteLine($"[SUCCESS] Result created with {result.Tokens.Count} tokens, {result.PurchaseRecords.Count} tour records, {result.BundlePurchaseRecords.Count} bundle records");

                Console.WriteLine("========== CHECKOUT COMPLETED SUCCESSFULLY ==========\n");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n========== CHECKOUT FAILED ==========");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
                Console.WriteLine("=====================================\n");
                throw;
            }
        }

        public List<TourPurchaseTokenDto> GetTokens(long touristId)
        {
            var tokens = _tokenRepository.GetByTouristId(touristId);
            return _mapper.Map<List<TourPurchaseTokenDto>>(tokens);
        }
    }
}
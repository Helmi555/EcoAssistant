// REMOVE AutoMapper using
// using AutoMapper;

using EcoAssistant.Application.DTOs;
using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;
using EcoAssistant.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EcoAssistant.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _repository;
        private readonly ILogger<PurchaseService> _logger;
        // REMOVE IMapper field

        public PurchaseService(
            IPurchaseRepository repository,
            // REMOVE IMapper parameter
            ILogger<PurchaseService> logger)
        {
            _repository = repository;
            // _mapper = mapper; // REMOVE this
            _logger = logger;
        }

        public async Task<PurchaseResponseDto> SavePurchaseAsync(PurchaseRequestDto purchaseDto, string userId)
        {
            try
            {
                // CHANGE: Use ToEntity() instead of AutoMapper
                var purchase = purchaseDto.ToEntity(userId);

                // Save to database
                var savedPurchase = await _repository.AddAsync(purchase);

                // Log the purchase
                _logger.LogInformation(
                    "Purchase saved for user {UserId}: {ProductName} (${Price})",
                    userId, purchase.ProductName, purchase.Price);

                // CHANGE: Use FromEntity() instead of AutoMapper
                return PurchaseResponseDto.FromEntity(savedPurchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving purchase for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<PurchaseResponseDto>> SaveCartPurchaseAsync(
            List<PurchaseRequestDto> purchaseDtos, string userId)
        {
            var savedPurchases = new List<PurchaseResponseDto>();

            foreach (var purchaseDto in purchaseDtos)
            {
                try
                {
                    var saved = await SavePurchaseAsync(purchaseDto, userId);
                    savedPurchases.Add(saved);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving cart item for user {UserId}", userId);
                    // Continue with other items
                }
            }

            return savedPurchases;
        }

        public async Task<List<PurchaseResponseDto>> GetUserPurchasesAsync(string userId)
        {
            var purchases = await _repository.GetByUserIdAsync(userId);

            // CHANGE: Use LINQ with FromEntity instead of AutoMapper
            return purchases
                .Select(PurchaseResponseDto.FromEntity)
                .ToList();
        }

        public async Task<PurchaseResponseDto?> GetPurchaseByIdAsync(int id)
        {
            var purchase = await _repository.GetByIdAsync(id);

            // CHANGE: Use ternary with FromEntity
            return purchase != null ? PurchaseResponseDto.FromEntity(purchase) : null;
        }
    }
}
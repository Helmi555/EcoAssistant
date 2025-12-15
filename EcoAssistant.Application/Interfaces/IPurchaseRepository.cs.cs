using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Domain.Interfaces
{
    // Simple repository interface for database operations
    public interface IPurchaseRepository
    {
        Task<ProductPurchase> AddAsync(ProductPurchase purchase);
        Task<ProductPurchase?> GetByIdAsync(int id);
        Task<List<ProductPurchase>> GetByUserIdAsync(string userId);
        Task<List<ProductPurchase>> GetAllAsync();
        Task UpdateAsync(ProductPurchase purchase);
        Task DeleteAsync(ProductPurchase purchase);
    }
}
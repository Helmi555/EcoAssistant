using EcoAssistant.Domain.Entities;
using EcoAssistant.Domain.Interfaces;
using EcoAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcoAssistant.Infrastructure.Repositories
{
    public class EfPurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext _context;

        public EfPurchaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductPurchase> AddAsync(ProductPurchase purchase)
        {
            await _context.ProductPurchases.AddAsync(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<ProductPurchase?> GetByIdAsync(int id)
        {
            return await _context.ProductPurchases.FindAsync(id);
        }

        public async Task<List<ProductPurchase>> GetByUserIdAsync(string userId)
        {
            return await _context.ProductPurchases
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();
        }

        public async Task<List<ProductPurchase>> GetAllAsync()
        {
            return await _context.ProductPurchases.ToListAsync();
        }

        public async Task UpdateAsync(ProductPurchase purchase)
        {
            _context.Entry(purchase).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductPurchase purchase)
        {
            _context.ProductPurchases.Remove(purchase);
            await _context.SaveChangesAsync();
        }
    }
}
using EcoAssistant.Application.DTOs;

namespace EcoAssistant.Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseResponseDto> SavePurchaseAsync(PurchaseRequestDto purchase, string userId);
        Task<List<PurchaseResponseDto>> SaveCartPurchaseAsync(List<PurchaseRequestDto> purchases, string userId);
        Task<List<PurchaseResponseDto>> GetUserPurchasesAsync(string userId);
        Task<PurchaseResponseDto?> GetPurchaseByIdAsync(int id);
    }
}
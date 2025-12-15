using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.DTOs
{
    // CHANGE from class to record with mapping methods
    public record PurchaseRequestDto(
        string ProductId,
        string ProductName,
        decimal Price,
        int Quantity,
        string Color,
        string Category,
        decimal CarbonFootprint,
        decimal WaterUsage,
        decimal EnergyUsage,
        decimal EcoScore,
        string Store = "Amazon",
        string ProductUrl = "")
    {
        // ADD this method (like your friend's ToEntity)
        public ProductPurchase ToEntity(string userId) =>
            new ProductPurchase
            {
                ProductId = this.ProductId,
                ProductName = this.ProductName,
                Price = this.Price,
                Quantity = this.Quantity,
                Color = this.Color,
                Category = this.Category,
                CarbonFootprint = this.CarbonFootprint,
                WaterUsage = this.WaterUsage,
                EnergyUsage = this.EnergyUsage,
                EcoScore = this.EcoScore,
                UserId = userId,
                PurchaseDate = DateTime.UtcNow,
                Store = this.Store,
                ProductUrl = this.ProductUrl
            };
    }

    // CHANGE from class to record with static method
    public record PurchaseResponseDto(
        int Id,
        string ProductName,
        decimal Price,
        int Quantity,
        decimal CarbonFootprint,
        decimal WaterUsage,
        decimal EnergyUsage,
        decimal EcoScore,
        DateTime PurchaseDate,
        string Category)
    {
        // ADD this static method (like your friend's FromEntity)
        public static PurchaseResponseDto FromEntity(ProductPurchase entity) =>
            new PurchaseResponseDto(
                entity.Id,
                entity.ProductName,
                entity.Price,
                entity.Quantity,
                entity.CarbonFootprint,
                entity.WaterUsage,
                entity.EnergyUsage,
                entity.EcoScore,
                entity.PurchaseDate,
                entity.Category
            );
    }

    // Optional: Keep as class or change to record
    public record CartPurchaseRequestDto(List<PurchaseRequestDto> Items);
}
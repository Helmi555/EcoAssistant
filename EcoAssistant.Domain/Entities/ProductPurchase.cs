using EcoAssistant.Domain.Common;

namespace EcoAssistant.Domain.Entities
{
    
    public class ProductPurchase : BaseAuditableEntity
    {
        // Required properties for your purchase tracking
        public string ProductId { get; set; } = string.Empty;       // Like ASIN for Amazon
        public string ProductName { get; set; } = string.Empty;     // Name of product
        public decimal Price { get; set; }                          // Price per item
        public int Quantity { get; set; } = 1;                      // How many bought
        public string Color { get; set; } = string.Empty;           // Optional: color
        public string Category { get; set; } = string.Empty;        // Electronics, Food, etc.

        // Carbon footprint data
        public decimal CarbonFootprint { get; set; }                // In kg CO₂
        public decimal WaterUsage { get; set; }                     // In liters
        public decimal EnergyUsage { get; set; }                    // In kWh
        public decimal EcoScore { get; set; }                       // 1-10 rating

        // User info
        public string UserId { get; set; } = string.Empty;          // Who bought it
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow; // When bought

        // Store info
        public string Store { get; set; } = "Amazon";               // Store name
        public string ProductUrl { get; set; } = string.Empty;      // Product URL
    }
}
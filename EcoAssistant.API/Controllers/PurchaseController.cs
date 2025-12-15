using EcoAssistant.Application.DTOs;
using EcoAssistant.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // User must be logged in
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ILogger<PurchasesController> _logger;

        public PurchasesController(
            IPurchaseService purchaseService,
            ILogger<PurchasesController> logger)
        {
            _purchaseService = purchaseService;
            _logger = logger;
        }

        // POST: api/purchases
        [HttpPost]
        public async Task<ActionResult<PurchaseResponseDto>> SavePurchase([FromBody] PurchaseRequestDto purchaseDto)
        {
            try
            {
                // Get the logged-in user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not logged in" });
                }

                // Save the purchase
                var result = await _purchaseService.SavePurchaseAsync(purchaseDto, userId);

                return Ok(new
                {
                    success = true,
                    message = "Purchase saved successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving purchase");
                return BadRequest(new
                {
                    success = false,
                    message = "Error saving purchase",
                    error = ex.Message
                });
            }
        }

        // POST: api/purchases/cart
        [HttpPost("cart")]
        public async Task<ActionResult> SaveCartPurchase([FromBody] CartPurchaseRequestDto cartDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (cartDto.Items == null || cartDto.Items.Count == 0)
                {
                    return BadRequest(new { message = "Cart is empty" });
                }

                var savedPurchases = await _purchaseService.SaveCartPurchaseAsync(cartDto.Items, userId);

                return Ok(new
                {
                    success = true,
                    message = $"{savedPurchases.Count} items saved successfully",
                    data = savedPurchases,
                    totalItems = savedPurchases.Count,
                    totalCarbon = savedPurchases.Sum(p => p.CarbonFootprint * p.Quantity)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving cart purchases");
                return BadRequest(new { message = "Error saving cart", error = ex.Message });
            }
        }

        // GET: api/purchases
        [HttpGet]
        public async Task<ActionResult> GetUserPurchases()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var purchases = await _purchaseService.GetUserPurchasesAsync(userId);

                // Calculate totals
                var totalCarbon = purchases.Sum(p => p.CarbonFootprint * p.Quantity);
                var totalSpent = purchases.Sum(p => p.Price * p.Quantity);
                var avgEcoScore = purchases.Any() ? purchases.Average(p => p.EcoScore) : 0;

                return Ok(new
                {
                    success = true,
                    data = purchases,
                    statistics = new
                    {
                        totalPurchases = purchases.Count,
                        totalCarbonFootprint = totalCarbon,
                        totalMoneySpent = totalSpent,
                        averageEcoScore = avgEcoScore
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching purchases");
                return BadRequest(new { message = "Error fetching purchases", error = ex.Message });
            }
        }

        // GET: api/purchases/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPurchaseById(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);

                if (purchase == null)
                {
                    return NotFound(new { message = "Purchase not found" });
                }

                return Ok(new
                {
                    success = true,
                    data = purchase
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching purchase {Id}", id);
                return BadRequest(new { message = "Error fetching purchase", error = ex.Message });
            }
        }
    }
}
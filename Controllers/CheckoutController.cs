using System.Security.Claims;
using ECommerceStore.Dto;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly ProductContext _context;
    private readonly ClaimsPrincipal _user;
    private readonly ILogger _logger;

    private readonly UserManager<IdentityUser> _userManager;

    private CheckoutService CheckoutService;

    public CheckoutController(ProductContext context,
        UserManager<IdentityUser> userManager, ClaimsPrincipal user, ILogger<ProductsController> logger)
    {
        _userManager = userManager;
        _context = context;
        _user = user;
        _logger = logger;
    }

    // POST: api/Checkout
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PurchaseResponse>> PostCheckout(PurchaseDto purchase)
    {
        //Auth: Authentication is required to place an order
        
        _logger.LogInformation(MyLogEvents.Checkout,"Checking out at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        CheckoutService = new CheckoutService(purchase, _context,
            _userManager, _user);

        try
        {
            // Retrieve the order with from the purchase data transfer object
            var order = await CheckoutService.Checkout();

            // Return a successful response
            return new PurchaseResponse(order.OrderTrackingNumber);
        }
        catch (Exception ex)
        {
            // If an exception occurs (e.g., product not found), return a bad request response
            
            _logger.LogWarning(MyLogEvents.CheckoutFailed, ex, "Failed to place order");
            
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }
}
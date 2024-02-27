using System.Security.Claims;
using ECommerceStore.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol;


namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ProductContext _context;

        private CheckoutService CheckoutService;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ClaimsPrincipal _user;
        
        public CheckoutController(ProductContext context,
            UserManager<IdentityUser> userManager, ClaimsPrincipal user)
        {
            _userManager = userManager;
            _context = context;
            _user = user;
        }

        // POST: api/Checkout
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PurchaseResponse>> PostCheckout(PurchaseDto purchase)
        {
            CheckoutService = new CheckoutService(purchase, _context,
                _userManager, _user);

            try
            {
                // Retrieve the order with from the purchase data transfer object
                Order order = await CheckoutService.Checkout();

                // Return a successful response
                return new PurchaseResponse(order.OrderTrackingNumber);
            }
            catch (Exception ex)
            {
                // If an exception occurs (e.g., product not found), return a bad request response
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }
    }
}
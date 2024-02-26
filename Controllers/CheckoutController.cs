using ECommerceStore.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceStore.Models;
using NuGet.Protocol;


namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ProductContext _context;

        private CheckoutService CheckoutService;

        public CheckoutController(ProductContext context)
        {
            _context = context;
        }

        // POST: api/Checkout
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PurchaseResponse>> PostCheckout(PurchaseDto purchase)
        {
            CheckoutService = new CheckoutService(purchase, _context);

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
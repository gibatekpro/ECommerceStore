using ECommerceStore.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceStore.Models;
using NuGet.Protocol;


namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly ProductContext _context;

        private CheckoutService CheckoutService;

        public OrderStatusController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/OrderStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrderStatus()
        {
            return await _context.OrderStatus.ToListAsync();
        }


    }
}
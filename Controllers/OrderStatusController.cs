using ECommerceStore.Dto;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Manager,Admin")]
public class OrderStatusController : ControllerBase
{
    
    //Auth: Only Manager and Admin should be able to perform
    //CRUD on Order Status
    
    private readonly ProductContext _context;
    private readonly ILogger _logger;

    private CheckoutService CheckoutService;
    

    public OrderStatusController(ProductContext context, ILogger<OrderStatusController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/OrderStatus
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrderStatus()
    {
        
        _logger.LogInformation(MyLogEvents.ListItems,"Getting All Order Status at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        return await _context.OrderStatus.ToListAsync();
    }
    
    // GET: api/OrderStatus/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderStatus>> GetOrderStatus(long id)
    {
                
        _logger.LogInformation(MyLogEvents.GetItem,"Getting Order Status by Status Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        var orderStatus = await _context.OrderStatus.FindAsync(id);

        if (orderStatus == null) return NotFound();

        return orderStatus;
    }

    // PUT: api/OrderStatus/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrderStatus(long id, OrderStatus orderStatus)
    {
        _logger.LogInformation(MyLogEvents.UpdateItem,
            "Updating Order Status with Order Status Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());
        
        if (id != orderStatus.Id) return BadRequest();

        _context.Entry(orderStatus).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderStatusExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/OrderStatus
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<OrderStatus>> PostOrderStatus(OrderStatus orderStatus)
    {
        _logger.LogInformation(MyLogEvents.InsertItem,
            "Inserting Order Status at {DT} ", 
            DateTime.UtcNow.ToLongTimeString());
        
        _context.OrderStatus.Add(orderStatus);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOrderStatus", new { id = orderStatus.Id }, orderStatus);
    }

    // DELETE: api/OrderStatus/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderStatus(long id)
    {
        
        _logger.LogInformation(MyLogEvents.DeleteItem,
            "Deleting Order Status with Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());
        
        var orderStatus = await _context.OrderStatus.FindAsync(id);
        if (orderStatus == null) return NotFound();

        _context.OrderStatus.Remove(orderStatus);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrderStatusExists(long id)
    {
        return _context.OrderStatus.Any(e => e.Id == id);
    }
}
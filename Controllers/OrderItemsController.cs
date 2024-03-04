using System.Net;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Manager,Admin")]
public class OrderItemsController : ControllerBase
{
    //Auth: ONLY Manager and Admin should be able to access the OrderItems Api directly
    //Auth: Users will only be able to see Order Items as part of their orders
    
    private readonly ProductContext _context;
    private readonly ILogger _logger;

    public OrderItemsController(ProductContext context, ILogger<OrderItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/OrderItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
    {
        _logger.LogInformation(MyLogEvents.ListItems,"Getting Order Items at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        
        return await _context.OrderItems.ToListAsync();
    }

    // GET: api/OrderItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItem>> GetOrderItem(long id)
    {
        _logger.LogInformation(MyLogEvents.GetItem,"Getting Order Item with id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());
        
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({Id}) NOT FOUND", id);

            return NotFound(new ExceptionHandler($"Order Item with Id = {id} Not Found"));
        }
        return orderItem;
    }

    // PUT: api/OrderItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderItem>> PutOrderItem(long id, OrderItem orderItem)
    {
        
        _logger.LogInformation(MyLogEvents.UpdateItem,"Updating Order Item with id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());
        
        if (id != orderItem.Id) return BadRequest();

        _context.Entry(orderItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderItemExists(id))
                
                _logger.LogWarning(MyLogEvents.GetItemNotFound, "OrderItem({Id}) NOT FOUND", id);

            return NotFound();
            throw;
        }
        
        return await _context.OrderItems.FindAsync(id) ?? throw new InvalidOperationException();
    }

    // POST: api/OrderItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)
    {
        
        _logger.LogInformation(MyLogEvents.InsertItem,"Inserting OrderItem at {DT} ", 
            DateTime.UtcNow.ToLongTimeString());

        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOrderItem", new { id = orderItem.Id }, orderItem);
    }

    // DELETE: api/OrderItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(long id)
    {
        _logger.LogInformation(MyLogEvents.DeleteItem,"Deleting OrderItem with Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());
        
        var orderItem = await _context.OrderItems.FindAsync(id);
        if (orderItem == null) return NotFound();

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        return Ok(new HttpResponseMessage(HttpStatusCode.OK));
    }

    private bool OrderItemExists(long id)
    {
        return _context.OrderItems.Any(e => e.Id == id);
    }
}
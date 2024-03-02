using System.Net;
using System.Security.Claims;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly ProductContext _context;
    private readonly ClaimsPrincipal _user;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger _logger;

    public OrdersController(ProductContext context, ClaimsPrincipal user, 
        UserManager<IdentityUser> userManager, ILogger<OrdersController> logger)
    {
        _context = context;
        _user = user;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: api/Orders
    [HttpGet]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        //Auth: Only Manager and Admin can fetch all Orders in database
        _logger.LogInformation(MyLogEvents.ListItems,"Getting All Orders at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        return await _context.Orders.ToListAsync();
    }

    // GET: api/Orders/GetUserOrders
    [HttpGet("GetUserOrders")]
    [Authorize(Roles = "Manager,Admin,User")]
    public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders([FromQuery] string userId)
    {
        //Auth: A user must only be able to fetch their own orders and NOT
        //orders belonging to other users
        _logger.LogInformation(MyLogEvents.ListItems,"Getting All Orders for specific user at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        //Now we fetch the Id of the authenticated user
        var authId = await GetAuthenticatedUserId(userId);
        
        _logger.LogInformation(MyLogEvents.ListItems,">>>>>Getting All Orders for specific user {authId} at {DT} ",
            authId, DateTime.UtcNow.ToLongTimeString());

        //Check if provided id belongs to the authenticated user
        if (authId.Equals(userId))
        {
            var orders = await _context.Orders
                .Where(o => o.UserProfile!.UserId == userId)
                .ToListAsync();
            if (!orders.Any())
            {
                _logger.LogWarning(MyLogEvents.GetItemNotFound, "No Orders for user with id = {userId}", userId);

                return NotFound(new ExceptionHandler($"No Orders for user with id = {userId}"));
            }
            return orders;
        }
        
        return Unauthorized(new ExceptionHandler($"Cannot access orders that belong to another user"));
    }

    // GET: api/Orders/FindByOrderStatusName
    [HttpGet("FindByOrderStatusName")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByOrderStatusName([FromQuery] string status)
    {
        //Auth: Authorisation required
        
        _logger.LogInformation(MyLogEvents.ListItems,"Getting Orders by Status name at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        
        var orders = await _context.Orders
            .Where(p => EF.Functions.Like(p.OrderStatus!.StatusName, $"%{status}%"))
            .ToListAsync();

        if (!orders.Any())
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({status}) NOT FOUND", status);

            return NotFound(new ExceptionHandler("No Orders match your search"));
        }

        return orders;
    }


    // GET: api/Orders/5
    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<Order>> GetOrder(long id)
    {
        //Auth: Only Manager and Admin can fetch a single Order in database
                
        _logger.LogInformation(MyLogEvents.GetItem,"Getting Order by Order Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());
        
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({id}) NOT FOUND", id);

            return NotFound(new ExceptionHandler($"Order with id = {id} not found"));
        }
        
        return order;
    }

    // PUT: api/Orders/5
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> PutOrder(long id, Order order)
    {
        //Auth: Only Manager and Admin can update a single Order in database
        _logger.LogInformation(MyLogEvents.UpdateItem,
            "Updating Product with Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        if (id != order.Id) return BadRequest();

        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // PUT: api/Orders/ChangeOrderStatus
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("ChangeOrderStatus")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<Order>> ChangeOrderStatus([FromQuery] long orderId, [FromQuery]long statusId)
    {
        //Auth: Only Manager and Admin can update the status of an order
        
        _logger.LogInformation(MyLogEvents.UpdateItem,
            "Updating Order Status Id = [{orderId}] with Status Id = [{statusId}] at {DT} ",
            orderId, statusId, DateTime.UtcNow.ToLongTimeString());

        var orderStatus = await _context.OrderStatus.FindAsync(statusId);
        
        if (orderStatus == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Order Status with ID = [{statusId}] NOT FOUND", statusId);

            return NotFound(new ExceptionHandler($"Product Category with Id = {statusId} Not Found"));
        }

        var order = await _context.Orders.FindAsync(orderId);
        
        if (order == null)
        {
            
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({orderId}) NOT FOUND", orderId);

            return NotFound(new ExceptionHandler("Order not found"));
        }

        order.OrderStatus = orderStatus;
        order.OrderStatusId = orderStatus.Id;
        
        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderExists(orderId))
                return NotFound();
            throw;
        }

        return order;
    }

    // POST: api/Orders
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<Order>> PostOrder(Order order)
    {
        //Auth: Only Manager and Admin can update a single Order in database
        //Users will only be able to create order through checkout
        
        _logger.LogInformation(MyLogEvents.InsertItem,"Inserting Order at {DT} ", 
            DateTime.UtcNow.ToLongTimeString());
        
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOrder", new { id = order.Id }, order);
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(long id)
    {
        //Auth: Only Manager and Admin can delete a single Order in database
        //Users will only not able to delete order 

                
        _logger.LogInformation(MyLogEvents.DeleteItem,"Deleting Order with Order Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new HttpResponseMessage(HttpStatusCode.OK));
    }

    private bool OrderExists(long id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
    
    private async Task<string> GetAuthenticatedUserId(string userId)
    {
        
        var userEmail = _user.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userEmail))
            // User is not authenticated or user ID claim is not found
            throw new Exception("User is not authenticated or user ID claim is not found");

        var user = await _userManager.FindByEmailAsync(userEmail);
        
        
        return user!.Id;
    }

}
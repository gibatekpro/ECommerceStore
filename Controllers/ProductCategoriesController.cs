using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductCategoriesController : ControllerBase
{
    private readonly ProductContext _context;
    private readonly ILogger _logger;

    public ProductCategoriesController(ProductContext context, ILogger<OrderStatusController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/ProductCategories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
    {
        _logger.LogInformation(MyLogEvents.ListItems,"Getting All Product Categories at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        
        return await _context.ProductCategories.ToListAsync();
    }

    // GET: api/ProductCategories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductCategory>> GetProductCategory(long id)
    {
        _logger.LogInformation(MyLogEvents.GetItem,"Getting Product Category by Category Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        
        var productCategory = await _context.ProductCategories.FindAsync(id);

        if (productCategory == null) return NotFound();

        return productCategory;
    }

    // PUT: api/ProductCategories/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> PutProductCategory(long id, ProductCategory productCategory)
    {
        
        _logger.LogInformation(MyLogEvents.UpdateItem,
            "Updating Product Category with Product Category Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        
        if (id != productCategory.Id) return BadRequest();

        _context.Entry(productCategory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductCategoryExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/ProductCategories
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategory productCategory)
    {
        
        _logger.LogInformation(MyLogEvents.InsertItem,
            "Inserting Product Category at {DT} ", 
            DateTime.UtcNow.ToLongTimeString());

        
        _context.ProductCategories.Add(productCategory);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProductCategory", new { id = productCategory.Id }, productCategory);
    }

    // DELETE: api/ProductCategories/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DeleteProductCategory(long id)
    {
        _logger.LogInformation(MyLogEvents.DeleteItem,
            "Deleting Product Category with Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        
        var productCategory = await _context.ProductCategories.FindAsync(id);
        if (productCategory == null) return NotFound();

        _context.ProductCategories.Remove(productCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductCategoryExists(long id)
    {
        return _context.ProductCategories.Any(e => e.Id == id);
    }
}
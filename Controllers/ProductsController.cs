using System.Net;
using System.Security.Claims;
using ECommerceStore.Dto;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ProductContext _context;
    private readonly EmailService _emailService;
    private readonly ILogger _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ClaimsPrincipal _user;

    public ProductsController(ProductContext context, UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, EmailService emailService, IConfiguration configuration,
        ILogger<ProductsController> logger, ClaimsPrincipal user)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _user = user;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        //Auth: Anyone can access. No auth required
        
        _logger.LogInformation(MyLogEvents.ListItems,"Getting Products at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        return await _context.Products.Include(p => p.ProductCategory).ToListAsync();
    }

    // GET: api/Products/FindByCategoryId
    [HttpGet("FindByCategoryId")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategoryId([FromQuery] long id)
    {
        //Auth: Anyone can access. No auth required
        
        _logger.LogInformation(MyLogEvents.ListItems,"Getting Products by Product Category Id at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        
        var productCategory = _context.ProductCategories
            .Where(c => c.Id == id);
        
        if (productCategory == null || !productCategory.Any())
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({Id}) NOT FOUND", id);

            return NotFound(new ExceptionHandler($"Product Category with Id = {id} Not Found"));
        }

        var products = await _context.Products
            .Where(p => p.ProductCategoryId == id)
            .ToListAsync();

        if (products == null || !products.Any())
        {
            
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({Id}) NOT FOUND", id);

            return NotFound(new ExceptionHandler("No Products in this Product Category"));
        }

        return products;
    }

    // GET: api/Products/FindByProductName
    [HttpGet("FindByProductName")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByProductName([FromQuery] string name)
    {
        //Auth: Anyone can access. No auth required
        
        _logger.LogInformation(MyLogEvents.ListItems,"Getting Products by name at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        
        var products = await _context.Products
            .Where(p => EF.Functions.Like(p.ProductName, $"%{name}%"))
            .ToListAsync();

        if (products == null || !products.Any())
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({name}) NOT FOUND", name);

            return NotFound(new ExceptionHandler("No Products match your search"));
        }

        return products;
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(long id)
    {
        //Auth: Anyone can access. No auth required
        
        _logger.LogInformation(MyLogEvents.GetItem,"Getting Product by Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());
        
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({id}) NOT FOUND", id);

            return NotFound(new ExceptionHandler($"Product with id = {id} not found"));
        }

        return product;
    }

    // PUT: api/Products/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<Product>> PutProduct(long id, ProductDto productDto)
    {
        //Auth: Only a Manager and Admin is allowed to update a product in the store

        
        _logger.LogInformation(MyLogEvents.UpdateItem,"Updating Product with Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        
        var product = new Product();
        try
        {
            product = await _context.Products.FindAsync(id);

            product = productDto.ProductUpdate(product);

            product.LastUpdated = DateTime.Now;

            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                _logger.LogWarning(MyLogEvents.UpdateItemNotFound, "Could not update Product Id = [{id}]", id);

                return NotFound(new ExceptionHandler($"Product with id = [{id}] not found"));
                
            }
            throw; 
        }

        return product;
    }
    

    // PUT: api/Products/ChangeProductCategory
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("ChangeProductCategory")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<Product>> ChangeProductCategory([FromQuery] long productId, [FromQuery]long categoryId)
    {
        //Auth: Only Manager and Admin can update the category of a product
        
        _logger.LogInformation(MyLogEvents.UpdateItem,
            "Updating Product Status Id = [{productId}] with Category Id = [{categoryId}] at {DT} ",
            productId, categoryId, DateTime.UtcNow.ToLongTimeString());

        var productCategory = await _context.ProductCategories.FindAsync(categoryId);
        
        if (productCategory == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Product Category with ID = [{categoryId}] NOT FOUND", categoryId);

            return NotFound(new ExceptionHandler($"Product Category with Id = {categoryId} Not Found"));
        }

        var product = await _context.Products.FindAsync(productId);
        
        if (product == null)
        {
            
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({productId}) NOT FOUND", productId);

            return NotFound(new ExceptionHandler("Product not found"));
        }

        product.ProductCategory = productCategory;
        product.ProductCategoryId = productCategory.Id;
        
        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(productId))
                return NotFound();
            throw;
        }

        return product;
    }


    // POST: api/Products
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<Product>> PostProduct(ProductDto productDto)
    {
        //Auth: Only a Manager and Admin is allowed to update a product in the store
        
        _logger.LogInformation(MyLogEvents.InsertItem,"Inserting Product at {DT} ", 
            DateTime.UtcNow.ToLongTimeString());

        
        var product = productDto.ToProduct();

        //The product was just created
        //and therefore just updated
        product.DateCreated = DateTime.Now;
        product.LastUpdated = DateTime.Now;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        //Auth: Only a Manager and Admin is allowed to update a product in the store
        
        
        _logger.LogInformation(MyLogEvents.DeleteItem,"Deleting Product with Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Product({id}) NOT FOUND", id);

            return NotFound(new ExceptionHandler("No Products match that id"));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new HttpResponseMessage(HttpStatusCode.OK));
    }

    private bool ProductExists(long id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
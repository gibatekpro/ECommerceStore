using ECommerceStore.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public ProductsController(ProductContext context,UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, EmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;  
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.ProductCategory).ToListAsync();
        }
        
        // GET: api/Products/FindByCategoryId
        [HttpGet("FindByCategoryId")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategoryId([FromQuery] long id)
        {
            var products = await _context.Products
                .Where(p => p.ProductCategoryId == id)
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return products;
        }
        
        // GET: api/Products/FindByProductName
        [HttpGet("FindByProductName")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByProductName([FromQuery] string name)
        {
            var products = await _context.Products
                .Where(p => EF.Functions.Like(p.ProductName, $"%{name}%"))
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<Product>> PutProduct(long id, ProductDto productDto)
        {
            Product? product = new Product();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return product;
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<Product>> PostProduct(ProductDto productDto)
        {
            Product product = productDto.ToProduct();
            
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
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

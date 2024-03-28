using System.Net;
using System.Security.Claims;
using AutoMapper;
using ECommerceStore.Dto;
using ECommerceStore.Mapper;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using NuGet.Protocol.Plugins;

namespace ECommerceStore.Controllers;

[DisableCors]
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
    private readonly ProductService _productService;
    private readonly PagedResponseProfile _pagedResponseProfile;
    private readonly IMapper _mapper;

    public ProductsController(ProductContext context, UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, EmailService emailService, IConfiguration configuration,
        ILogger<ProductsController> logger, ClaimsPrincipal user, ProductService productService,
        PagedResponseProfile pagedResponseProfile,
        IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _user = user;
        _productService = productService;
        _pagedResponseProfile = pagedResponseProfile;
        _mapper = mapper;
    }

    // // GET: api/Products
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    // {
    //     //Auth: Anyone can access. No auth required
    //     
    //     _logger.LogInformation(MyLogEvents.ListItems,"Getting Products at {DT} ",
    //         DateTime.UtcNow.ToLongTimeString());
    //     return await _context.Products.Include(p => p.ProductCategory).ToListAsync();
    // }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsPaginated(
        [FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        //Auth: Anyone can access. No auth required

        _logger.LogInformation(MyLogEvents.ListItems, "Getting Products at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        if (page <= 0 || size <= 0)
            return BadRequest($"{nameof(page)} and {nameof(size)} size must be greater than 0.");

        PagedResponse<Product> pagedProducts;

        try
        {
            pagedProducts =
                await _productService.GetProductsPaginated(page, size);
        }
        catch (ExceptionHandler e)
        {
            return BadRequest(new { message = e.Message });
        }

        var pagedProductsDto =
            _mapper.Map<PagedResponseDto<Product>>(pagedProducts);

        return Ok(pagedProductsDto);
    }

    // GET: api/Products
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(long id)
    {
        //Auth: Anyone can access. No auth required

        _logger.LogInformation(MyLogEvents.GetItem, "Getting Product by Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        Product product;

        try
        {
            product = await _productService.GetProductById(id);
        }
        catch (ExceptionHandler e)
        {
            return BadRequest(new { message = e.Message });
        }

        var productDto =
            _mapper.Map<ProductDto>(product);

        return Ok(productDto);
    }


    // GET: api/Products/search/findByProductCategoryId
    [HttpGet("search/findByProductCategoryId")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategoryId(
        [FromQuery] long id, [FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        //Auth: Anyone can access. No auth required

        _logger.LogInformation(MyLogEvents.ListItems, "Getting Products by Product Category Id at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        if (page <= 0 || size <= 0)
            return BadRequest($"{nameof(page)} and {nameof(size)} size must be greater than 0.");

        PagedResponse<Product> pagedProducts;

        try
        {
            pagedProducts =
                await _productService.GetProductsByProductCategoryIdPaginated(id, page, size);
        }
        catch (ExceptionHandler e)
        {
            return BadRequest(new { message = e.Message });
        }

        var pagedProductsDto =
            _mapper.Map<PagedResponseDto<Product>>(pagedProducts);

        return Ok(pagedProductsDto);
    }

    // GET: api/Products/search/findByNameContainsIgnoreCase
    [HttpGet("search/findByNameContainsIgnoreCase")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByProductName([FromQuery] string name,
        int page = 1, int size = 20)
    {
        //Auth: Anyone can access. No auth required

        _logger.LogInformation(MyLogEvents.ListItems, "Getting Products by name at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        if (page <= 0 || size <= 0)
            return BadRequest($"{nameof(page)} and {nameof(size)} size must be greater than 0.");

        PagedResponse<Product> pagedProducts;

        try
        {
            pagedProducts =
                await _productService.GetProductsByProductName(name, page, size);
        }
        catch (ExceptionHandler e)
        {
            return BadRequest(new { message = e.Message });
        }

        var pagedProductsDto =
            _mapper.Map<PagedResponseDto<Product>>(pagedProducts);

        return Ok(pagedProductsDto);
    }

    // PUT: api/Products
    [HttpPut]
    [Authorize(Roles = "SuperAdmin,Manager,Admin")]
    public async Task<ActionResult<Product>> PutProduct([FromQuery] long id, [FromBody] ProductDto productUpdate)
    {
        //Auth: Only a SuperAdmin, Manager and Admin is allowed to update a product in the store


        _logger.LogInformation(MyLogEvents.UpdateItem, "Updating Product with Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        Product product;
        try
        {
            product = await _productService.UpdateProduct(id, productUpdate);
        }
        catch (ExceptionHandler e)
        {
            return BadRequest(new { message = e.Message });
        }

        var productDto = _mapper.Map<ProductDto>(product);

        return Ok(productDto);
    }


    // PUT: api/Products/ChangeProductCategory
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("changeProductCategory")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin")]
    public async Task<ActionResult<Product>> ChangeProductCategory([FromQuery] long productId,
        [FromQuery] long categoryId)
    {
        //Auth: Only SuperAdmin, Manager and Admin can update the category of a product

        _logger.LogInformation(MyLogEvents.UpdateItem,
            "Updating Product Status Id = [{productId}] with Category Id = [{categoryId}] at {DT} ",
            productId, categoryId, DateTime.UtcNow.ToLongTimeString());

        Product product;

        try
        {
            product = await _productService.UpdateProductCategory(productId, categoryId);
        }
        catch (ExceptionHandler e)
        {
            return BadRequest(new { message = e.Message });
        }

        var productDto = _mapper.Map<ProductDto>(product);

        return Ok(productDto);
    }


    // POST: api/Products
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Manager,Admin")]
    public async Task<ActionResult<Product>> PostProduct(ProductDto productUpdate)
    {
        //Auth: Only a SuperAdmin, Manager and Admin is allowed to update a product in the store

        _logger.LogInformation(MyLogEvents.InsertItem, "Inserting Product at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        var productFromDto = _mapper.Map<Product>(productUpdate);

        Product product;

        try
        {
            product = await _productService.PostProduct(productFromDto);
        }
        catch (ExceptionHandler e)
        {
            return BadRequest(new { message = e.Message });
        }

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        //Auth: Only a SuperAdmin, Manager and Admin is allowed to update a product in the store

        _logger.LogInformation(MyLogEvents.DeleteItem, "Deleting Product with Product Id = [{id}] at {DT} ",
            id, DateTime.UtcNow.ToLongTimeString());

        try
        {
            await _productService.DeleteProduct(id);
        }
        catch (ExceptionHandler e)
        {
            return NotFound(new { message = e.Message });
        }

        return Ok(new HttpResponseMessage(HttpStatusCode.OK));
    }
}
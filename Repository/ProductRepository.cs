using System.Net;
using System.Security.Claims;
using ECommerceStore.Controllers;
using ECommerceStore.Dto;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Repository;

public class ProductRepository(ProductContext context, UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager, EmailService emailService, IConfiguration configuration,
    ILogger<ProductsController> logger, ClaimsPrincipal user)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ProductContext _context = context;
    private readonly EmailService _emailService = emailService;
    private readonly ILogger _logger = logger;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly ClaimsPrincipal _user = user;


    public async Task<PagedResponse<Product>> GetProductsPaginated(
        int number, int size)
    {
        var totalElements = await _context.Products.AsNoTracking().CountAsync();

        var products = await _context.Products.AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((number - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalPages = (totalElements + size - 1) / size;

        _logger.LogInformation(MyLogEvents.ListItems, "Getting Products at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        var pagedResponse = new PagedResponse<Product>(new Embedded<Product>(products),
            new Page(size, totalElements, totalPages, number));

        return pagedResponse;
    }

    public async Task<PagedResponse<Product>> GetProductsByProductCategoryIdPaginated(
        long id, int number, int size)
    {
        var productCategory = _context.ProductCategories
            .Where(c => c.Id == id);

        if (productCategory == null || !productCategory.Any())
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({Id}) NOT FOUND", id);

            throw new ExceptionHandler($"Product Category with Id = {id} Not Found");
        }


        var products = await _context.Products.AsNoTracking()
            .Where(p => p.ProductCategoryId == id)
            .OrderBy(x => x.Id)
            .Skip((number - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalElements = await _context.Products
            .Where(p => p.ProductCategoryId == id)
            .CountAsync();

        var totalPages = (totalElements + size - 1) / size;

        _logger.LogInformation(MyLogEvents.ListItems, "Getting Products at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        var pagedResponse = new PagedResponse<Product>(new Embedded<Product>(products),
            new Page(size, totalElements, totalPages, number));

        return pagedResponse;
    }

    public async Task<PagedResponse<Product>> GetProductsByProductName(
        string name, int number, int size)
    {
        var products = await _context.Products.AsNoTracking()
            .Where(p => EF.Functions.Like(p.ProductName, $"%{name}%"))
            .OrderBy(x => x.Id)
            .Skip((number - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalElements = await _context.Products
            .Where(p => EF.Functions.Like(p.ProductName, $"%{name}%"))
            .CountAsync();

        var totalPages = (totalElements + size - 1) / size;

        _logger.LogInformation(MyLogEvents.ListItems, "Getting Products at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        var pagedResponse = new PagedResponse<Product>(new Embedded<Product>(products),
            new Page(size, totalElements, totalPages, number));

        return pagedResponse;
    }

    public async Task<Product> GetProductById(long id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({id}) NOT FOUND", id);

            throw new ExceptionHandler($"Product with id = {id} not found");
        }

        return product;
    }

    public async Task<Product> UpdateProduct(long id, ProductDto productDto)
    {
        var product = new Product();
        

        try
        {
            product = await _context.Products.FindAsync(id);

            if (!ProductExists(id))
            {
                _logger.LogWarning(MyLogEvents.UpdateItemNotFound, "Could not update Product Id = [{id}]", id);

                throw new ExceptionHandler($"Product with id = {id} not found");
            }

            product = productDto.ProductUpdate(product);

            product.LastUpdated = DateTime.Now;

            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            if (!ProductExists(id))
            {
                _logger.LogWarning(MyLogEvents.UpdateItemNotFound, "Could not update Product Id = [{id}]", id);

                throw new ExceptionHandler($"Product with id = [{id}] not found");
            }

            throw new ExceptionHandler(e.Message);
        }

        return product;
    }


    public async Task<Product> UpdateProductCategory(long productId, long categoryId)
    {
        var productCategory = await _context.ProductCategories.FindAsync(categoryId);

        if (productCategory == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Product Category with ID = [{categoryId}] NOT FOUND",
                categoryId);

            throw new ExceptionHandler($"Product Category with Id = {categoryId} Not Found");
        }

        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({productId}) NOT FOUND", productId);

            throw new ExceptionHandler("Product not found");
        }

        product.ProductCategory = productCategory;
        product.ProductCategoryId = productCategory.Id;


        try
        {
            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            if (!ProductExists(productId))
                throw new ExceptionHandler(e.Message);
        }

        return product;
    }

    public async Task<Product> PostProduct(Product product)
    {
        //The product was just created
        //and therefore just updated
        product.DateCreated = DateTime.Now;
        product.LastUpdated = DateTime.Now;

        try
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ExceptionHandler(e.Message);
        }


        return product;
    }

    public async Task DeleteProduct(long id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "Product({id}) NOT FOUND", id);

            throw new ExceptionHandler("No Products match that id");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    private bool ProductExists(long id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
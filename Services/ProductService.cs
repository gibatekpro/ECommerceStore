using System.Security.Claims;
using ECommerceStore.Controllers;
using ECommerceStore.Dto;
using ECommerceStore.Models;
using ECommerceStore.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Services;

public class ProductService(ProductRepository productRepository)
{
    private readonly ProductRepository _productRepository = productRepository;

    public async Task<PagedResponse<Product>> GetProductsPaginated(
        int pageNumber, int pageSize)
    {
        return await _productRepository
            .GetProductsPaginated(pageNumber, pageSize);
    }
    public async Task<PagedResponse<Product>> GetProductsByProductCategoryIdPaginated(
        long id, int pageNumber, int pageSize)
    {
        return await _productRepository
            .GetProductsByProductCategoryIdPaginated(id, pageNumber, pageSize);
    }
    
    
    public async Task<PagedResponse<Product>> GetProductsByProductName(
        string name, int pageNumber, int pageSize)
    {
        return await _productRepository
            .GetProductsByProductName(name, pageNumber, pageSize);
    }

    public async Task<Product> GetProductById(long id)
    {
        return await _productRepository.GetProductById(id);
    }

    public async Task<Product> UpdateProduct(long id, ProductDto productUpdate)
    {
        return await _productRepository.UpdateProduct(id, productUpdate);
    }

    public async Task<Product> UpdateProductCategory(long productId, long categoryId)
    {
        return await _productRepository.UpdateProductCategory(productId, categoryId);
    }

    public async Task<Product> PostProduct(Product product)
    {
        return await _productRepository.PostProduct(product);
    }

    public async Task DeleteProduct(long id)
    {
        await _productRepository.DeleteProduct(id);
    }
}
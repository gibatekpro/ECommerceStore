using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public class ProductDto
{
    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public double? UnitPrice { get; set; }

    public string? ImageUrl { get; set; }

    public bool? Active { get; set; }

    public int? UnitsInStock { get; set; }

    public long? ProductCategoryId { get; set; }

    public Product ProductUpdate(Product product)
    {
        product.ProductName = ProductName ?? product.ProductName;
        product.Description = Description ?? product.Description;
        product.UnitPrice = UnitPrice ?? product.UnitPrice;
        product.ImageUrl = ImageUrl ?? product.ImageUrl;
        product.Active = Active ?? product.Active;
        product.UnitsInStock = UnitsInStock ?? product.UnitsInStock;

        // Should not be able to update categoryId from here
        // product.ProductCategoryId = ProductCategoryId != null ? ProductCategoryId.Value : product.ProductCategoryId;

        return product;
    }
}
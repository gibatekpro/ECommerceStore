using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public class ProductDto
{
    public string ProductName { get; set; }

    public string? Description { get; set; }

    public double UnitPrice { get; set; }

    public string? ImageUrl { get; set; }

    public bool? Active { get; set; }

    public int? UnitsInStock { get; set; }

    public long ProductCategoryId { get; set; }

    public Product ToProduct()
    {
        return new Product
        {
            ProductName = ProductName,
            Description = Description,
            UnitPrice = UnitPrice,
            ImageUrl = ImageUrl,
            Active = Active,
            UnitsInStock = UnitsInStock,
            ProductCategoryId = ProductCategoryId
        };
    }

    public Product ProductUpdate(Product product)
    {
        product.ProductName = ProductName;
        product.Description = Description;
        product.UnitPrice = UnitPrice;
        product.ImageUrl = ImageUrl;
        product.Active = Active;
        product.UnitsInStock = UnitsInStock;
        product.ProductCategoryId = ProductCategoryId;

        return product;
    }
}
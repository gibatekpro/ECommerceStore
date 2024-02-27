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
            ProductName = this.ProductName,
            Description = this.Description,
            UnitPrice = this.UnitPrice,
            ImageUrl = this.ImageUrl,
            Active = this.Active,
            UnitsInStock = this.UnitsInStock,
            ProductCategoryId = this.ProductCategoryId
        };
    }
    public Product ProductUpdate(Product product)
    {

        product.ProductName = this.ProductName;
        product.Description = this.Description;
        product.UnitPrice = this.UnitPrice;
        product.ImageUrl = this.ImageUrl;
        product.Active = this.Active;
        product.UnitsInStock = this.UnitsInStock;
        product.ProductCategoryId = this.ProductCategoryId;

        return product;
    }
}

using System.Text.Json.Serialization;

namespace ECommerceStore.Models{
public class Product
{
    public long Id { get; set; }

    public string ProductName { get; set; }

    public string? Description { get; set; }

    public double UnitPrice { get; set; }

    public string? ImageUrl { get; set; }

    public bool? Active { get; set; }

    public int? UnitsInStock { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? LastUpdated { get; set; }

    public long ProductCategoryId { get; set; }
    
    [JsonIgnore]
    public ProductCategory ProductCategory { get; set; }

    }

}
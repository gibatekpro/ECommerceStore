
namespace ECommerceStore.Models{

    public class ProductCategory
    {
        public long Id { get; set; }

        public string CategoryName { get; set; }

        public ICollection<Product> ProductList { get; set; }
    }
    
}


namespace ECommerceStore.Models
{
    public class OrderItem
    {

        public long Id { get; set; }

        public string ImageUrl { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public long ProductId { get; set; }

        public long? OrderId { get; set; }

        public Order? Order { get; set; }
    }
}


namespace ECommerceStore.Models
{
    public class OrderItem
    {

        public long Id { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public long ProductId { get; set; }

        public long? OrderId { get; set; }

        public Order? Order { get; set; }
    }
}

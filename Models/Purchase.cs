
namespace ECommerceStore.Models
{
	public class Purchase
	{
        public Customer Customer { get; set; }

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }

        public Order Order { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}


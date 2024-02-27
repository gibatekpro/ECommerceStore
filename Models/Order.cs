
using System.Text.Json.Serialization;

namespace ECommerceStore.Models
{
    public class Order
    {

        public long Id { get; set; }

        public string? OrderTrackingNumber { get; set; }

        public int TotalQuantity { get; set; }

        public double TotalPrice { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastUpdated { get; set; }

        public long? UserProfileId { get; set; }

        [JsonIgnore]
        public UserProfile? UserProfile { get; set; }

        public long? ShippingAddressId { get; set; }
        
        [JsonIgnore]
        public Address? ShippingAddress { get; set; }

        public long? BillingAddressId { get; set; }

        [JsonIgnore]
        public Address? BillingAddress { get; set; }
       
        public long? OrderStatusId { get; set; }
        
        [JsonIgnore]
        public OrderStatus? OrderStatus { get; set; }

        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

    }
}

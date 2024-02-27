using System.Text.Json.Serialization;

namespace ECommerceStore.Models

{
    public class OrderStatus
    {
        public long Id { get; set; }

        public string StatusName { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
        
    } 
}
namespace ECommerceStore.Models

{
    public class OrderStatus
    {
        public long Id { get; set; }

        public string StatusName { get; set; }

        public ICollection<Order> Orders { get; set; }
        
    } 
}
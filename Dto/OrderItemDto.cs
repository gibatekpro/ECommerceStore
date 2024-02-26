using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public class OrderItemDto
{
    
    public int Quantity { get; set; }

    public long ProductId { get; set; }

    public OrderItem ToOrderItem()
    {
        return new OrderItem
        {
            Quantity = this.Quantity,
            
            ProductId = this.ProductId
        };
    }

}
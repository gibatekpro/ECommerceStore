using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public class PurchaseDto
{
    public CustomerDto Customer { get; set; }
    
    public AddressDto ShippingAddress { get; set; }

    public AddressDto BillingAddress { get; set; }

    public ICollection<OrderItemDto> OrderItems { get; set; }
}
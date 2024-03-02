namespace ECommerceStore.Models;

public class PurchaseResponse
{
    public PurchaseResponse(string OrderTrackingNumber)
    {
        this.OrderTrackingNumber = OrderTrackingNumber;
    }

    public string OrderTrackingNumber { get; set; }
}
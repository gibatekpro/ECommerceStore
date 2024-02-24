using System;
namespace ECommerceStore.Models
{
	public class PurchaseResponse
	{

        public String OrderTrackingNumber { get; set; }

        public PurchaseResponse(String OrderTrackingNumber)
		{
			this.OrderTrackingNumber = OrderTrackingNumber;
		}


    }
}


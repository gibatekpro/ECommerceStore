using Swashbuckle.AspNetCore.Filters;

namespace ECommerceStore.Models
{
    public class PurchaseExample : IExamplesProvider<Purchase>
    {
        public Purchase GetExamples()
        {
            return new Purchase
            {
                UserProfile = new UserProfile
                {
                    FirstName = "Tony",
                    LastName = "Gibah",
                },
                ShippingAddress = new Address
                {
                    Street = "Wembley",
                    City = "Brent",
                    State = "London",
                    Country = "United Kingdom",
                    ZipCode = "HA9 0FR"
                },
                BillingAddress = new Address
                {
                    Street = "Wembley",
                    City = "Brent",
                    State = "London",
                    Country = "United Kingdom",
                    ZipCode = "HA9 0FR"
                },
                Order = new Order
                {
                    TotalPrice = 36.98,
                    TotalQuantity = 2
                },
                OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Quantity = 1,
                            UnitPrice = 18.99,
                            ProductId = 26
                        },
                        new OrderItem
                        {
                            Quantity = 1,
                            UnitPrice = 17.99,
                            ProductId = 51
                        }
                    }
            };
        }
    }
}


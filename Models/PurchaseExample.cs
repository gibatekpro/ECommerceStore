using Swashbuckle.AspNetCore.Filters;

namespace ECommerceStore.Models
{
    public class PurchaseExample : IExamplesProvider<Purchase>
    {
        public Purchase GetExamples()
        {
            return new Purchase
            {
                Customer = new Customer
                {
                    FirstName = "Tony",
                    LastName = "Gibah",
                    Email = "tony@test.com"
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
                    TotalPrice = 36.98m,
                    TotalQuantity = 2
                },
                OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ImageUrl = "assets/images/products/coffeemugs/coffeemug.png",
                            Quantity = 1,
                            UnitPrice = 18.99m,
                            ProductId = 26
                        },
                        new OrderItem
                        {
                            ImageUrl = "assets/images/products/mousepads/mousepad.png",
                            Quantity = 1,
                            UnitPrice = 17.99m,
                            ProductId = 51
                        }
                    }
            };
        }
    }
}


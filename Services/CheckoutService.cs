using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Dto;

public class CheckoutService(PurchaseDto _PurchaseDto, ProductContext _Context)
{
    private PurchaseDto _PurchaseDto = _PurchaseDto;

    private ProductContext _Context = _Context;
    
    public async Task<Order> Checkout()
    {
        Order order = await GetOrder();

        //Add order to Customer
        await AddOrderToCustomer(order);

        return order;
    }

    private async Task<Order> GetOrder() {
        return new Order
        {
            OrderTrackingNumber = GenerateOrderTrackingNumber(),
            TotalQuantity = CalculateTotalQuantity(),
            TotalPrice = await CalculateTotalPrice(),
            DateCreated = DateTime.Now,
            LastUpdated = DateTime.Now,
            ShippingAddress = _PurchaseDto.ShippingAddress.ToAddress(),
            BillingAddress = _PurchaseDto.BillingAddress.ToAddress(),
            OrderStatus = await SetOrderStatus(),
            OrderItems = await GetOrderItems()
        };
    }

    private async Task<ICollection<OrderItem>> GetOrderItems()
    {
        // Create a new list to store OrderItem objects
        List<OrderItem> orderItems = new List<OrderItem>();

        // Iterate over each OrderItemDto in purchase.OrderItems
        foreach (OrderItemDto orderItemDto in _PurchaseDto.OrderItems)
        {
            // Convert OrderItemDto to OrderItem using ToOrderItem() method
            OrderItem orderItem = orderItemDto.ToOrderItem();

            var productFromDB = await _Context.Products
                .FirstOrDefaultAsync(p => p.Id == orderItem.ProductId);

            if (productFromDB != null)
            {
                orderItem.UnitPrice = productFromDB.UnitPrice;
            }

            else
            {
                // If the product is not found, throw an exception
                throw new Exception($"Product with ID {orderItem.ProductId} not found.");
            }

            // Add the converted OrderItem to the list
            orderItems.Add(orderItem);
        }

        // Convert the list to ICollection<OrderItem> and return
        return orderItems;
    }

    private async Task<OrderStatus?> SetOrderStatus()
    {
        //Get Order status "Ordered"
        return await _Context.OrderStatus
            .FirstOrDefaultAsync(c => c.StatusName == "Ordered");
    }

    private async Task<double> CalculateTotalPrice()
    {
        double totalPrice = 0;
        foreach (var orderItem in _PurchaseDto.OrderItems)
        {
            var productFromDB = await _Context.Products
                .FirstOrDefaultAsync(p => p.Id == orderItem.ProductId);

            if (productFromDB != null)
            {
                totalPrice += productFromDB.UnitPrice;
            }
            else
            {
                // If the product is not found, throw an exception
                throw new Exception($"Product with ID {orderItem.ProductId} not found.");
            }
        }

        return totalPrice;
    }

    private int CalculateTotalQuantity()
    {
        int totalQuantity = 0;
        foreach (var orderItem in _PurchaseDto.OrderItems)
        {
            totalQuantity += orderItem.Quantity;
        }

        return totalQuantity;
    }

    private string GenerateOrderTrackingNumber()
    {
        // Generate a random UUID (GUID version-4)
        return Guid.NewGuid().ToString();
    }
    
    private async Task AddOrderToCustomer(Order order)
    {
            
        //populate customer with order
        Customer customer = _PurchaseDto.Customer.ToCustomer();

        // check if this is an existing customer
        String email = customer.Email;

        var customerFromDB = await _Context.Customers
            .FirstOrDefaultAsync(c => c.Email == email);

        if (customerFromDB != null)
        {
            // we found them... let's assign them accordingly
            customer = customerFromDB;

            //update the customer
            _Context.Entry(customer).State = EntityState.Modified;
                
            //Add order to customer
            customer.Add(order);
        }
        else
        {
            //Not found...Now we add the order to the customer
            _Context.Customers.Add(customer);
                
            //Add order to customer
            customer.Add(order);

        }

        try
        {
            await _Context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        
    }


}
using System.Security.Claims;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Dto;

public class CheckoutService
{
    private PurchaseDto _PurchaseDto;
    private readonly ProductContext _Context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ClaimsPrincipal _user;
    
    public CheckoutService(PurchaseDto purchaseDto, 
        ProductContext context,UserManager<IdentityUser> userManager, 
        ClaimsPrincipal user)
    {
        _Context = context;
        _userManager = userManager;
        _PurchaseDto = purchaseDto;
        _user = user;
    }


    public async Task<Order> Checkout()
    {
        
        Order order = await GetOrder();

        _Context.Orders.Add(order);

        try
        {
            await _Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return order;
    }

    private async Task<Order> GetOrder()
    {
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
            UserProfile = await SetUserProfile(),
            OrderItems = SetOrderItems(),
        };
    }

    private ICollection<OrderItem> SetOrderItems()
    {
        ICollection<OrderItem> orderItems = new List<OrderItem>();
        
        foreach (var orderItem in _PurchaseDto.OrderItems)
        {
            orderItems.Add(orderItem.ToOrderItem());
        }
        
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
            var productFromDb = await _Context.Products
                .FirstOrDefaultAsync(p => p.Id == orderItem.ProductId);

            if (productFromDb != null)
            {
                totalPrice += productFromDb.UnitPrice;
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

        try
        {
            
            foreach (var orderItem in _PurchaseDto.OrderItems)
            {
                totalQuantity += orderItem.Quantity;
            }
        }
        catch (Exception e)
        {
            throw new Exception($"YUUUUPPDPDPPD");
        }


        return totalQuantity;
    }

    private string GenerateOrderTrackingNumber()
    {
        // Generate a random UUID (GUID version-4)
        return Guid.NewGuid().ToString();
    }

    private async Task<UserProfile> SetUserProfile()
    {
        var userEmail = _user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userEmail))
        {
            // User is not authenticated or user ID claim is not found
            throw new Exception("User is not authenticated or user ID claim is not found");
        }
        
        var user = await _userManager.FindByEmailAsync(userEmail);
        var userId =  user?.Id;
        
        //UserProfile
        UserProfile userProfile = _PurchaseDto.UserProfile.ToUserProfile();
        
        // Save authenticated user's Id on Profile table
        userProfile.Email = userEmail;
        userProfile.UserId = userId;

        //Check if profile already exists to avoid creating it again
        var userProfileFromDb = await _Context.UserProfiles
            .FirstOrDefaultAsync(c => c.Email == userProfile.Email);

        return userProfileFromDb ?? userProfile;
    }

}
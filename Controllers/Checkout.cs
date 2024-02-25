using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceStore.Models;



namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ProductContext _context;

        public CheckoutController(ProductContext context)
        {
            _context = context;
        }

        // POST: api/Checkout
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PurchaseResponse>> PostCheckout(Purchase purchase)
        {

            //retrieve the order info from purchase
            Order order = purchase.Order;
            //generate tracking number
            String orderTrackingNumber = GenerateOrderTrackingNumber();
            order.OrderTrackingNumber = orderTrackingNumber;
            
            //Add Date Created and Last Updated
            order.DateCreated = DateTime.Now;
            order.LastUpdated = DateTime.Now;
            
            //Set Order status to "Ordered"
            var orderStatusFromDB = await _context.OrderStatus
                .FirstOrDefaultAsync(c => c.StatusName == "Ordered");
            if (orderStatusFromDB != null)
            {
                order.OrderStatusId = orderStatusFromDB.Id;
            }

            //populate order with orderItems
            // Populate order with orderItems
            ICollection<OrderItem> orderItems = purchase.OrderItems;
            foreach (var item in orderItems)
            {
                order.Add(item);
            }

            //populate order with billingAddress and shippingAddress
            order.BillingAddress = purchase.BillingAddress;
            order.ShippingAddress = purchase.ShippingAddress;

            //populate customer with order
            Customer customer = purchase.Customer;

            // check if this is an existing customer
            String email = customer.Email;

            var customerFromDB = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customerFromDB != null)
            {
                // we found them... let's assign them accordingly
                customer = customerFromDB;

                customer.Add(order);

                //update the customer
                _context.Entry(customer).State = EntityState.Modified;
            }
            else
            {
                //Not found...Now we add the order to the customer
                _context.Customers.Add(customer);

                customer.Add(order);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return new PurchaseResponse(orderTrackingNumber);
        }

        private string GenerateOrderTrackingNumber()
        {
            // Generate a random UUID (GUID version-4)
            return Guid.NewGuid().ToString();
        }

    }
}


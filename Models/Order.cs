using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerceStore.Models;

namespace ECommerceStore.Models
{
    public class Order
    {

        public long Id { get; set; }

        public string? OrderTrackingNumber { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalPrice { get; set; }


        public string? Status { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastUpdated { get; set; }

        public long CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public long? ShippingAddressId { get; set; }

        
        public Address? ShippingAddress { get; set; }

        public long? BillingAddressId { get; set; }

        /*
         * 
         * If the key is not nullable the related object 
         * must be deleted, and circular relations don't 
         * allow that. So use nullable foreign key.
         * 
         * **/
        
        public Address? BillingAddress { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public void Add(OrderItem item)
        {
            if (item != null)
            {
                OrderItems.Add(item);
                item.Order = this;
            }
        }
    }
}

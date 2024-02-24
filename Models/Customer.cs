using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerceStore.Models;

namespace ECommerceStore.Models
{
    public class Customer
    {

        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<Order> Orders { get; set; }

        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public void Add(Order order)
        {
            if (order != null)
            {
                Orders.Add(order);
                order.CustomerId = Id;
            }
        }
    }
}

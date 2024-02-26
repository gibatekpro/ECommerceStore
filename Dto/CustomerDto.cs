using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public class CustomerDto
{

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public Customer ToCustomer()
    {
        return new Customer
        {
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email

        };
    }
    
}

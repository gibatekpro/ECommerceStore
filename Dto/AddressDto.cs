using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public class AddressDto
{
    public string City { get; set; }

    public string Country { get; set; }

    public string State { get; set; }

    public string Street { get; set; }

    public string ZipCode { get; set; }

    public Address ToAddress()
    {
        return new Address
        {
            City = this.City,
            Country = this.Country,
            State = this.State,
            Street = this.Street,
            ZipCode = this.ZipCode
        };
    }
}
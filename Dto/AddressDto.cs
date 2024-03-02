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
            City = City,
            Country = Country,
            State = State,
            Street = Street,
            ZipCode = ZipCode
        };
    }
}
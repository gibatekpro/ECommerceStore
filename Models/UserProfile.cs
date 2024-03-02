namespace ECommerceStore.Models;

public class UserProfile
{
    public UserProfile()
    {
        Orders = new HashSet<Order>();
    }

    public long Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? UserId { get; set; }

    public string Email { get; set; }

    public ICollection<Order> Orders { get; set; }
}
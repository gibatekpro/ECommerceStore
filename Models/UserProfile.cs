
namespace ECommerceStore.Models
{
    public class UserProfile
    {

        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string? UserId { get; set; }

        public string Email { get; set; }

        public ICollection<Order> Orders { get; set; }
        
        public UserProfile()
        {
            Orders = new HashSet<Order>();
        }

    }
}

using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public class UserProfileDto
{

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserProfile ToUserProfile()
    {
        return new UserProfile
        {
            FirstName = this.FirstName,
            LastName = this.LastName,

        };
    }
    
}

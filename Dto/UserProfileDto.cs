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
            FirstName = FirstName,
            LastName = LastName
        };
    }

    public UserProfile UserProfileUpdate(UserProfile userProfile)
    {
        userProfile.FirstName = FirstName;
        userProfile.LastName = LastName;

        return userProfile;
    }
}
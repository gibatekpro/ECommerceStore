namespace ECommerceStore.Models;

public class LoginResponse
{
    
    public string Message { get; set; }
    
    public string UserId { get; set; }
    
    public string Email { get; set; }
    
    public string Token { get; set; }

    public LoginResponse(string message, string userId, string email, string token)
    {
        Message = message;

        UserId = userId;

        Email = email;

        Token = token;
    } 
}
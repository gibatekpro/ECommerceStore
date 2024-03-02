using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger _logger;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        EmailService emailService, IConfiguration configuration, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthModel model)
    {
        //Auth: Anyone can access. No auth required
        
        _logger.LogInformation(MyLogEvents.RegisteringAccount,"Registering account at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Generate an email verification token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create the verification link
            var verificationLink =
                Url.Action("VerifyEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

            // Send the verification email
            var emailSubject = "Email Verification";
            var emailBody = $"Please verify your email by clicking the following link: {verificationLink}";
            _emailService.SendEmail(user.Email, emailSubject, emailBody);
            
            _logger.LogInformation(MyLogEvents.RegisteringAccount,"Registered account successfully at {DT} ",
                DateTime.UtcNow.ToLongTimeString());

            return Ok("User registered successfully. An email verification link has been sent.");
        }

        _logger.LogWarning(MyLogEvents.AuthenticationFailed, "Could not register");
        return BadRequest(result.Errors);
    }


    // Add an action to handle email verification
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        //Auth: Anyone can access. No auth required
        
        _logger.LogInformation(MyLogEvents.VerifyingAccount,"Verifying account at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return NotFound("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            //Email Confirmed. Add user to Role "User"
            await _userManager.AddToRoleAsync(user, RoleType.User.ToString());

            _logger.LogInformation(MyLogEvents.VerifyingAccount,"Verifying account at {DT} ",
                DateTime.UtcNow.ToLongTimeString());
            
            return Ok("Email verification successful. Proceed to log in");
        }

        _logger.LogWarning(MyLogEvents.AuthenticationFailed,"Failed to Verify Email at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        
        return BadRequest("Email verification failed.");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthModel model)
    {        
        
        //Auth: Anyone can access. No auth required
        
        _logger.LogInformation(MyLogEvents.SigningInAccount,"Login in account at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            
            _logger.LogInformation(MyLogEvents.SigningInAccount,"Login successful at {DT}. Jwt Token generated: {token}  ",
                DateTime.UtcNow.ToLongTimeString(), token);
            
            return Ok(new LoginResponse("Login successful", user.Id, user.Email!, token));
        }
        
        _logger.LogWarning(MyLogEvents.AuthenticationFailed,"Failed to log in at {DT} ",
            DateTime.UtcNow.ToLongTimeString());
        
        return Unauthorized("Invalid login attempt.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        
        _logger.LogInformation(MyLogEvents.SigningOutAccount,"Log out at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        return Ok("Logged out");
    }

    private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add roles as claims
        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"]));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
}
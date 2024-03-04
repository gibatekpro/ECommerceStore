using ECommerceStore.Dto;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProfilesController : ControllerBase
{
    private readonly ProductContext _context;
    private readonly ILogger _logger;

    public UserProfilesController(ProductContext context, ILogger<OrderStatusController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/UserProfiles
    [HttpGet]
    [Authorize(Roles = "SuperAdmin, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
    {
        
        _logger.LogInformation(MyLogEvents.ListItems,"Getting All User Profiles at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        
        var userProfiles = await _context.UserProfiles.ToListAsync();

        // Iterate through each userProfiles
        foreach (var userProfile in userProfiles)
        {
            // Retrieve orders for the current userProfile
            var orders = await _context.Orders
                .Where(o => o.UserProfileId == userProfile.Id)
                .ToListAsync();

            // Add the orders to the current userProfile
            userProfile.Orders = orders;
        }

        return userProfiles;
    }

    // GET: api/UserProfiles/5
    [HttpGet("{id}")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,User")]
    public async Task<ActionResult<UserProfile>> GetUserProfile(long id)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);

        if (userProfile == null) return NotFound();

        return userProfile;
    }

    // PUT: api/UserProfiles/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,User")]
    public async Task<IActionResult> PutUserProfile(long id, UserProfileDto userProfileDto)
    {
        try
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);

            if (userProfile != null) userProfile = userProfileDto.UserProfileUpdate(userProfile);

            _context.Entry(userProfile).State = EntityState.Modified;


            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserProfileExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/UserProfiles
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "SuperAdmin, Manager, Admin")]
    public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
    {
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUserProfile", new { id = userProfile.Id }, userProfile);
    }

    // DELETE: api/UserProfiles/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin, Manager, Admin")]
    public async Task<IActionResult> DeleteUserProfile(long id)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);
        if (userProfile == null) return NotFound();

        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserProfileExists(long id)
    {
        return _context.UserProfiles.Any(e => e.Id == id);
    }
}
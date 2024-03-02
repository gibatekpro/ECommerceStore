using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceStore.Controllers;

[Route("api/[controller]")]

//Only the Manager (Created when program is initially run
//will be allowed to perform CRUD on Roles.
//This will ensure that only the Manager can higher Admins
[ApiController]
[Authorize(Roles = "Manager")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger _logger;

    public RolesController(RoleManager<IdentityRole> roleManager, 
        UserManager<IdentityUser> userManager, ILogger<RolesController> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetRoles()
    {
        _logger.LogInformation(MyLogEvents.ListItems,"Getting All Roles at {DT} ",
            DateTime.UtcNow.ToLongTimeString());

        
        var roles = _roleManager.Roles.ToList();
        return Ok(roles);
    }

    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetRole(string roleId)
    {
        
        _logger.LogInformation(MyLogEvents.GetItem,"Getting Role by Role Id = [{roleId}] at {DT} ",
            roleId, DateTime.UtcNow.ToLongTimeString());

        
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null) return NotFound("Role not found.");

        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var role = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded) return Ok("Role created successfully.");

        return BadRequest(result.Errors);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
    {
        var role = await _roleManager.FindByIdAsync(model.RoleId);

        if (role == null) return NotFound("Role not found.");

        role.Name = model.NewRoleName;
        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded) return Ok("Role updated successfully.");

        return BadRequest(result.Errors);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRole(string roleId)
    {

        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null) return NotFound("Role not found.");

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded) return Ok("Role deleted successfully.");

        return BadRequest(result.Errors);
    }

    [HttpPost("assign-role-to-user")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
    {
        _logger.LogInformation(MyLogEvents.GetItem,"Assigning role to user at {DT} ", 
            DateTime.UtcNow.ToLongTimeString());
        
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null) return NotFound("User not found.");

        var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

        if (!roleExists) return NotFound("Role not found.");

        var result = await _userManager.AddToRoleAsync(user, model.RoleName);

        if (result.Succeeded) return Ok("Role assigned to user successfully.");

        return BadRequest(result.Errors);
    }

    [HttpPut("remove-role-from-user")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleModel model)
    {
        _logger.LogInformation(MyLogEvents.GetItem,"Removing role from user at {DT} ", 
            DateTime.UtcNow.ToLongTimeString());
        
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null) return NotFound("User not found.");

        var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

        if (!roleExists) return NotFound("Role not found.");

        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

        if (result.Succeeded) return Ok("Role removed from user successfully.");

        return BadRequest(result.Errors);
    }
}
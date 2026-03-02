using System.Security.Claims;
using blog_community_api.Contracts.Users;
using blog_community_api.Core.Interfaces;
using blog_community_api.Core.Services;
using blog_community_api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace blog_community_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    
    public UsersController(IService<User> userService)
    {
        _userService = userService as UserService ?? throw new Exception("UserService is unavailable.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser([FromRoute] Guid id)
    {
        var user = await _userService.GetAsync(id);
        if (user is null) return NotFound();
        
        return Ok(user);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername([FromRoute] string username)
    {
        var user = await _userService.GetByUsernameAsync(username);
        if (user is null) return NotFound();
        
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _userService.RegisterUserAsync(request);
        if (result.Error is not null) return BadRequest(result.Error);
        
        return CreatedAtAction(nameof(GetUser), new { id = result.Response?.Id }, result.Response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var result = await _userService.LoginUserAsync(request);
        if (result.Error is not null) return BadRequest(result.Error);
        
        return Ok(result.Response);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty || currentUserId != id) return Unauthorized();
        

        var updated = await _userService.UpdateUserAsync(id, request);
        if (updated.Error is not null) return BadRequest(updated.Error);
        
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty || currentUserId != id) return Unauthorized();

        var deleted = await _userService.DeleteAsync(id);
        
        if (!deleted) return NotFound();

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdValue = HttpContext.User.Claims.FirstOrDefault(claim =>
            claim.Type is ClaimTypes.NameIdentifier or JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(userIdValue?.Value, out var userId)
            ? userId
            : Guid.Empty;
    }
}
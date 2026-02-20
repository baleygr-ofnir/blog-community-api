using AutoMapper;
using blog_community_api.Contracts.Users;
using blog_community_api.Data.Entities;
using blog_community_api.Data.Repositories;
using blog_community_api.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace blog_community_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserRepository? _userRepository;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;
    
    public UsersController(IRepository<User> userRepository, IMapper mapper, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository as UserRepository;
        _mapper = mapper;
        _jwtTokenService = jwtTokenService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        if (_userRepository is null) return StatusCode(StatusCodes.Status500InternalServerError, "User repository not available.");
        
        User? user = await _userRepository.GetAsync(id);
        if (user is null) return NotFound();
        
        UserResponse? response = _mapper.Map<UserResponse>(user);
        return Ok(response);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername([FromRoute] string username)
    {
        if (_userRepository is null) return StatusCode(StatusCodes.Status500InternalServerError, "User repository not available.");
        
        User? user = await _userRepository.GetByUsernameAsync(username);
        if (user is null) return NotFound();

        UserResponse? response = _mapper.Map<UserResponse>(user);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        if (_userRepository is null) return StatusCode(StatusCodes.Status500InternalServerError, "User repository not available.");
        
        var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUsername is not null) return BadRequest("Username already exists");
        
        var existingEmail = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingEmail is not null) return BadRequest("Email already exists");
        
        var user = _mapper.Map<User>(request);
        
        user.PasswordHash = PasswordHasher.HashPassword(request.Password);
        
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = null;
        
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        var response = _mapper.Map<UserResponse>(user);
        
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        if (_userRepository is null)
            return StatusCode(StatusCodes.Status500InternalServerError, "User repository not available");

        var user = await _userRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);
        if (user is null) return Unauthorized("Invalid username/email or password.");

        var isValidPassword = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isValidPassword) return Unauthorized("Invalid/email or password.");

        var token = _jwtTokenService.GenerateToken(user);

        var response = new UserLoginResponse()
        {
            UserId = user.Id,
            Token = token,
        };
        
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
    {
        if (_userRepository is null) return StatusCode(StatusCodes.Status500InternalServerError, "User repository not available.");
        
        User? user = await _userRepository.GetAsync(id);
        if (user is null) return NotFound();

        if (!string.Equals(user.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUsername is not null && existingUsername.Id != id)
                return BadRequest("Username is already taken.");
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingEmail = await _userRepository.GetUserByEmailAsync(request.Email);

            if (existingEmail is not null && existingEmail.Id != id) return BadRequest("Email is already registered");
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;
        
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        UserResponse? response = _mapper.Map<UserResponse>(user);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (_userRepository is null) return StatusCode(StatusCodes.Status500InternalServerError, "User repository not available.");
        bool deleted = await _userRepository.Delete(id);

        if (!deleted) return NotFound();

        await _userRepository.SaveChangesAsync();
        return NoContent();
    }
}
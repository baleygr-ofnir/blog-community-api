using AutoMapper;
using blog_community_api.Contracts.Users;
using blog_community_api.Data.Entities;
using blog_community_api.Data.Repositories;
using blog_community_api.Security;

namespace blog_community_api.Core.Services;

public class UserService : GenericService<User>
{
    private readonly UserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public UserService(IRepository<User> userRepository, IMapper mapper, IJwtTokenService jwtTokenService) : base(
        userRepository, mapper)
    {
        _userRepository = userRepository as UserRepository ?? throw new ArgumentException("UserRepository required");
        _jwtTokenService = jwtTokenService;
    }

    public async Task<UserResponse?> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        return user is null ? null : Mapper.Map<UserResponse>(user);
    }

    public async Task<(UserResponse? Response, string? Error)> RegisterUserAsync(UserRegisterRequest request)
    {
        var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUsername is not null) return (null, "Username is already registered.");

        var existingEmail = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingEmail is not null) return (null, "Email is already registered.");

        var user = Mapper.Map<User>(request);
        user.PasswordHash = PasswordHasher.HashPassword(request.Password);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = null;

        var added = await AddAsync(user);

        var response = Mapper.Map<UserResponse>(added);

        return (response, null);
    }

    public async Task<(UserLoginResponse? Response, string? Error)> LoginUserAsync(UserLoginRequest request)
    {
        var user = await _userRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);
        if (user is null) return (null, "Invalid credentials.");

        var validPassword = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!validPassword) return (null, "Invalid credentials");

        var token = _jwtTokenService.GenerateToken(user);
        
        var response = new UserLoginResponse
        {
            UserId = user.Id,
            Token = token
        };

        return (response, null);
    }

    public async Task<(UserResponse? Response, string? Error)> UpdateUserAsync(Guid id, UserUpdateRequest request)
    {
        var user = await Repository.GetAsync(id);
        if (user is null) return (null, "User not found.");

        if (!string.Equals(user.Username, request.Username, StringComparison.Ordinal))
        {
            var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUsername is not null && existingUsername.Id != id) return (null, "Username is already registered.");
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.Ordinal))
        {
            var existingEmail = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingEmail is not null && existingEmail.Id != id) return (null, "Email is already registered.");
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;
        
        var updated = await Update(id, user);

        var response = Mapper.Map<UserResponse>(user);

        return (response, null);
    }
}
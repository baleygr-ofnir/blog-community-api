namespace blog_community_api.Contracts.Users;

public class UserLoginRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}
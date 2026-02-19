namespace blog_community_api.Contracts.Users;

public class UserLoginResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
}
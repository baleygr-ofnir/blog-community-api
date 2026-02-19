namespace blog_community_api.Contracts.Users;

public class UserUpdateRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}
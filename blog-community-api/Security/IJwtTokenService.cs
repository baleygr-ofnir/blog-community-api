using blog_community_api.Data.Entities;

namespace blog_community_api.Security;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
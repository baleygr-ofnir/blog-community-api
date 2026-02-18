using blog_community_api.Entities;

namespace blog_community_api.Data.Repositories;

public class UserRepository : GenericRepository<User>
{
    public UserRepository(BlogContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var user = await FindAsync(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        return user.FirstOrDefault();
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await FindAsync(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        return user.FirstOrDefault();
    }
}
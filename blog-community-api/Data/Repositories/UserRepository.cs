using blog_community_api.Data.Entities;

namespace blog_community_api.Data.Repositories;

public class UserRepository : GenericRepository<User>
{
    public UserRepository(BlogContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var user = await FindAsync(u => u.Username.ToLower() == username.ToLower());
        return user.FirstOrDefault();
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await FindAsync(u => u.Email.ToLower() == email.ToLower());
        return user.FirstOrDefault();
    }

    public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        var user = await FindAsync
            (
                u =>
                    u.Username.ToLower() == usernameOrEmail.ToLower()
                    || u.Email.ToLower() ==  usernameOrEmail.ToLower()
            );
        
        return user.FirstOrDefault();
    }
}
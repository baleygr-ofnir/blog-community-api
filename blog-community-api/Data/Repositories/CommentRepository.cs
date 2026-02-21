using System.Linq.Expressions;
using blog_community_api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace blog_community_api.Data.Repositories;

public class CommentRepository(BlogContext context) : GenericRepository<Comment>(context)
{
    public override async Task<Comment?> GetAsync(Guid id)
    {
        return await Context.Comments
            .Include(c => c.User)
            .Include(c => c.BlogPost)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IEnumerable<Comment>> AllAsync()
    {
        return await Context.Comments
            .Include(c => c.User)
            .Include(c => c.BlogPost)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<IEnumerable<Comment>> FindAsync(Expression<Func<Comment, bool>> predicate)
    {
        return await Context.Comments
            .Where(predicate)
            .Include(c => c.User)
            .Include(c => c.BlogPost)
            .AsNoTracking()
            .ToListAsync();
    }
}
using System.Linq.Expressions;
using blog_community_api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace blog_community_api.Data.Repositories;

public class BlogPostRepository(BlogContext context) : GenericRepository<BlogPost>(context)
{
    public override async Task<BlogPost?> GetAsync(Guid id)
    {
        return await Context.BlogPosts
            .Include(bp => bp.Category)
            .Include(bp => bp.User)
            .FirstOrDefaultAsync(bp => bp.Id == id);
    }

    public override async Task<IEnumerable<BlogPost>> AllAsync()
    {
        return await Context.BlogPosts
            .Include(bp => bp.Category)
            .Include(bp => bp.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<IEnumerable<BlogPost>> FindAsync(Expression<Func<BlogPost, bool>> predicate)
    {
        return await Context.BlogPosts
            .Where(predicate)
            .Include(bp => bp.Category)
            .Include(bp => bp.User)
            .AsNoTracking()
            .ToListAsync();
    }
}
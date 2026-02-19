using blog_community_api.Data.Entities;

namespace blog_community_api.Data.Repositories;

public class BlogPostRepository : GenericRepository<BlogPost>
{
    public BlogPostRepository(BlogContext context) : base(context)
    {
    }
}
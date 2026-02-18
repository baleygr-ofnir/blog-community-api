using blog_community_api.Entities;

namespace blog_community_api.Data.Repositories;

public class CommentRepository : GenericRepository<Comment>
{
    public CommentRepository(BlogContext context) : base(context)
    {
    }
}
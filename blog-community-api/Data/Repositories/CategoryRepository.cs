using blog_community_api.Entities;

namespace blog_community_api.Data.Repositories;

public class CategoryRepository : GenericRepository<Category>
{
    public CategoryRepository(BlogContext context) : base(context)
    {
    }
}
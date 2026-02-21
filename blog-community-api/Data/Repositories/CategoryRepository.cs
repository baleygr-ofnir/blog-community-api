using blog_community_api.Data.Entities;

namespace blog_community_api.Data.Repositories;

public class CategoryRepository(BlogContext context) : GenericRepository<Category>(context);
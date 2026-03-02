using AutoMapper;
using blog_community_api.Contracts.BlogPosts;
using blog_community_api.Data.Entities;
using blog_community_api.Data.Repositories;

namespace blog_community_api.Core.Services;

public class CategoryService : GenericService<Category>
{
    public CategoryService(IRepository<Category> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync()
    {
        var categories = await AllAsync();
        var response = Mapper.Map<List<CategoryResponse>>(categories);
        
        return response;
    }

    public async Task<CategoryResponse?> GetCategoryAsync(Guid id)
    {
        var category = await GetAsync(id);
        var response = Mapper.Map<CategoryResponse>(category);

        return category is not null ? response : null;
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        var category = Mapper.Map<Category>(request);
        category.Id = Guid.NewGuid();

        var added = await AddAsync(category);
        var response = Mapper.Map<CategoryResponse>(added);

        return response;
    }

    public async Task<bool> UpdateAsync(Guid id, CategoryRequest request)
    {
        var category = await GetAsync(id);
        if (category is null) return false;

        Mapper.Map(request, category);

        var updated = await Update(id, category);

        return updated is not null;
    }
}
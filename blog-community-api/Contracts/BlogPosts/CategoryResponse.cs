namespace blog_community_api.Contracts.BlogPosts;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
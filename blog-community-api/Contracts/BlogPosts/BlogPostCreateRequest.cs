namespace blog_community_api.Contracts.BlogPosts;

public class BlogPostCreateRequest
{
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}
namespace blog_community_api.Contracts.BlogPosts;

public class BlogPostResponse
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string AuthorUsername { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
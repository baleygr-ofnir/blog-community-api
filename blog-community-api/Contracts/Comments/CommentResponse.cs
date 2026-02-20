namespace blog_community_api.Contracts.Comments;

public class CommentResponse
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public string AuthorUsername { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
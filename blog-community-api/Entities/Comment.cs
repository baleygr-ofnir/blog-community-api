namespace blog_community_api.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public BlogPost BlogPost { get; set; } = null!;
    public User User { get; set; } = null!;

}
namespace blog_community_api.Entities;

public class BlogPost
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
namespace blog_community_api.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
using AutoMapper;
using blog_community_api.Contracts.BlogPosts;
using blog_community_api.Contracts.Comments;
using blog_community_api.Data.Entities;
using blog_community_api.Data.Repositories;

namespace blog_community_api.Core.Services;

public class BlogPostService : GenericService<BlogPost>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Comment> _commentRepository;
    
    public BlogPostService(IRepository<BlogPost> repository, IMapper mapper, IRepository<Category> categoryRepository, IRepository<Comment> commentRepository) : base(repository, mapper)
    {
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
    }

    public async Task<(BlogPostResponse? Response, string? Error)> CreateAsync(Guid userId, BlogPostCreateRequest request)
    {
        var category = await _categoryRepository.GetAsync(request.CategoryId);
        if (category is null) return (null, "Category was not found.");

        var blogPost = Mapper.Map<BlogPost>(request);
        blogPost.Id = Guid.NewGuid();
        blogPost.UserId = userId;
        blogPost.CreatedAt = DateTime.UtcNow;

        var added = await AddAsync(blogPost);

        var response = Mapper.Map<BlogPostResponse>(added);

        return (response, null);
    }

    public async Task<IEnumerable<BlogPostResponse>> SearchAsync(Guid? categoryId, string? title)
    {
        IEnumerable<BlogPost> blogPosts;

        if (categoryId.HasValue || !string.IsNullOrWhiteSpace(title))
        {
            var trimmedTitle = title?.Trim();
            blogPosts = await FindAsync
            (
                blogPost =>
                    (!categoryId.HasValue || blogPost.CategoryId == categoryId.Value)
                    && (
                        string.IsNullOrEmpty(trimmedTitle)
                        || blogPost.Title.ToLower().Contains(trimmedTitle.ToLower()))
            );
        }
        else
        {
            blogPosts = await AllAsync();
        }

        var response = Mapper.Map<List<BlogPostResponse>>(blogPosts.ToList());

        return response;
    }

    public async Task<BlogPostResponse?> GetBlogPostAsync(Guid id)
    {
        var blogPost = await GetAsync(id);

        var response = Mapper.Map<BlogPostResponse>(blogPost);

        return blogPost is not null ? response : null;
    }

    public async Task<(bool isUpdated, string? Error)> UpdateAsync(Guid id, BlogPostUpdateRequest request)
    {
        var blogPost = await GetAsync(id);
        if (blogPost is null) return (false, "Blog post was not found.");

        var category = await _categoryRepository.GetAsync(request.CategoryId);
        if (category is null) return (false, "Category was not found.");

        Mapper.Map(request, blogPost);
        blogPost.UpdatedAt = DateTime.UtcNow;

        var updated = await Update(id, blogPost);

        var response = updated is not null;
        
        return (response, null);
    }

    public async Task<(CommentResponse? Response, string? Error)> CreateCommentAsync(Guid blogPostId, Guid userId, CommentCreateRequest request)
    {
        var blogPost = await GetAsync(blogPostId);
        if (blogPost is null) return (null, "Blog post was not found.");

        var comment = Mapper.Map<Comment>(request);
        comment.Id = Guid.NewGuid();
        comment.BlogPostId = blogPostId;
        comment.UserId = userId;
        comment.CreatedAt = DateTime.UtcNow;

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var created = await _commentRepository.GetAsync(comment.Id);
        if (created is null) return (null, "Comment creation unsuccessful.");

        var response = Mapper.Map<CommentResponse>(created);

        return (response, null);
    }

    public async Task<(IEnumerable<CommentResponse>? Response, string? Error)> GetCommentsAsync(Guid blogPostId)
    {
        var blogPost = await GetAsync(blogPostId);
        if (blogPost is null) return (null, "Blog post was not found.");

        var comments = await _commentRepository.FindAsync(comment => comment.BlogPostId == blogPostId);

        var response = Mapper.Map<List<CommentResponse>>(comments.ToList());

        return (response, null);
    }
}
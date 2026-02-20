using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using blog_community_api.Contracts.BlogPosts;
using blog_community_api.Contracts.Comments;
using blog_community_api.Data;
using blog_community_api.Data.Entities;
using blog_community_api.Data.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;

namespace blog_community_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly IRepository<BlogPost> _blogPostRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Comment> _commentRepository;
    private readonly IMapper _mapper;

    public BlogPostsController
    (
        IRepository<BlogPost> blogPostRepository,
        IRepository<Category> categoryRepository,
        IRepository<Comment> commentRepository,
        IMapper mapper
    )
    {
        _blogPostRepository = blogPostRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BlogPostResponse>> CreateBlogPost([FromBody] BlogPostCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var category = await _categoryRepository.GetAsync(request.CategoryId);
        if (category is null) return BadRequest("Category not found");

        var newBlogPost = _mapper.Map<BlogPost>(request);
        newBlogPost.Id = Guid.NewGuid();
        newBlogPost.UserId = userId;
        newBlogPost.CreatedAt = DateTime.UtcNow;

        await _blogPostRepository.AddAsync(newBlogPost);
        await _blogPostRepository.SaveChangesAsync();

        var createdBlogPost = await _blogPostRepository.GetAsync(newBlogPost.Id);
        if (createdBlogPost is null)
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load created blog post.");

        var response = _mapper.Map<BlogPostResponse>(createdBlogPost);
        return CreatedAtAction(nameof(GetBlogPostById), new { id = createdBlogPost.Id }, response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BlogPostResponse>>> GetBlogPosts
    (
        [FromQuery] Guid? categoryId,
        [FromQuery] string? title
    )
    {
        IEnumerable<BlogPost> blogPosts;

        if (categoryId.HasValue || !string.IsNullOrWhiteSpace(title))
        {
            var trimmedTitle = title?.Trim();
            blogPosts = await _blogPostRepository.FindAsync
                (
                    bp => 
                        (!categoryId.HasValue || bp.CategoryId == categoryId.Value)
                        && (string.IsNullOrEmpty(title) || bp.Title.ToLower().Contains(title.ToLower()))
                );
        }
        else
        {
            blogPosts = await _blogPostRepository.AllAsync();
        }
        
        var responses = _mapper.Map<List<BlogPostResponse>>(blogPosts.ToList());
        return Ok(responses);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BlogPostResponse>> GetBlogPostById(Guid id)
    {
        var blogPost = await _blogPostRepository.GetAsync(id);
        if (blogPost is null) return NotFound();
        
        var response = _mapper.Map<BlogPostResponse>(blogPost);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateBlogPost
    (
        Guid id,
        [FromBody] BlogPostUpdateRequest request
    )
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();
        
        var blogPost = await _blogPostRepository.GetAsync(id);
        if (blogPost is null) return NotFound();
        
        if (blogPost.UserId != userId) return Forbid();
        
        var matchingCategories = await _categoryRepository.GetAsync(request.CategoryId);
        if (matchingCategories is null) return BadRequest("Invalid category id.");
        
        _mapper.Map(request, blogPost);
        blogPost.UpdatedAt = DateTime.UtcNow;
        
        _blogPostRepository.Update(blogPost);
        await _blogPostRepository.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteBlogPost(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();
        
        var blogPost = await _blogPostRepository.GetAsync(id);
        if (blogPost is null) return NotFound();
        
        if (blogPost.UserId != userId) return Forbid();

        var wasDeleted = await _blogPostRepository.Delete(id);
        if (!wasDeleted) return NotFound();
        
        await _blogPostRepository.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPost("{blogPostId:guid}/comments")]
    [Authorize]
    public async Task<ActionResult<CommentResponse>> CreateComment(Guid blogPostId, [FromBody] CommentCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();
        
        var blogPost = await _blogPostRepository.GetAsync(blogPostId);
        if (blogPost is null) return NotFound("BlogPost not found");
        
        if (blogPost.UserId == userId) return Forbid();

        var comment = _mapper.Map<Comment>(request);
        comment.Id = Guid.NewGuid();
        comment.BlogPostId = blogPostId;
        comment.UserId = userId;
        comment.CreatedAt = DateTime.UtcNow;
        
        await _commentRepository.AddAsync(comment);
        await _blogPostRepository.SaveChangesAsync();
        
        var createdComment = await _commentRepository.GetAsync(comment.Id);
        if (createdComment is null) return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load created comment.");
        
        var response = _mapper.Map<CommentResponse>(createdComment);
        
        return CreatedAtAction
            (
                nameof(GetBlogPostById),
                new { id = createdComment.Id },
                response
            );
    }

    [HttpGet("{blogPostId:guid}/comments")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetComments(Guid blogPostId)
    {
        var blogPost = await _blogPostRepository.GetAsync(blogPostId);
        if (blogPost is null) return NotFound("Blog post not found.");
        
        var comments = await _commentRepository.FindAsync(c => c.BlogPostId == blogPostId);
        var responses = _mapper.Map<List<CommentResponse>>(comments);
        
        return Ok(responses);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = HttpContext.User.Claims
            .FirstOrDefault(c =>
                c.Type is ClaimTypes.NameIdentifier or JwtRegisteredClaimNames.Sub);
        
        return Guid.TryParse(userIdClaim?.Value, out var userId)
            ? userId
            : Guid.Empty;
    }
}

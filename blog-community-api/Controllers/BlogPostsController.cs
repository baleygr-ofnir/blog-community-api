using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using blog_community_api.Contracts.BlogPosts;
using blog_community_api.Contracts.Comments;
using blog_community_api.Core.Interfaces;
using blog_community_api.Core.Services;
using blog_community_api.Data.Entities;
using Microsoft.IdentityModel.JsonWebTokens;

namespace blog_community_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly BlogPostService _blogPostService;
    
    public BlogPostsController(IService<BlogPost> blogPostService)
    {
        _blogPostService = blogPostService as BlogPostService ?? throw new Exception("BlogPostService is unavailable.");
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BlogPostResponse>> CreateBlogPost([FromBody] BlogPostCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _blogPostService.CreateAsync(userId, request);
        if (result.Error is not null) return BadRequest(result.Error);
        
        return CreatedAtAction(nameof(GetBlogPost), new { id = result.Response?.Id }, result.Response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BlogPostResponse>>> GetBlogPosts
    (
        [FromQuery] Guid? categoryId,
        [FromQuery] string? title
    )
    {
        var result = await _blogPostService.SearchAsync(categoryId, title);
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BlogPostResponse>> GetBlogPost([FromRoute] Guid id)
    {
        var result = await _blogPostService.GetBlogPostAsync(id);
        
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateBlogPost ([FromRoute] Guid id, [FromBody] BlogPostUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var updated = await _blogPostService.UpdateAsync(id, request);
        if (updated.Error is not null) return BadRequest(updated.Error);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var deleted = await _blogPostService.DeleteAsync(id);
        if (!deleted) return NotFound();
        
        return NoContent();
    }

    [HttpPost("{blogPostId:guid}/comments")]
    [Authorize]
    public async Task<ActionResult<CommentResponse>> CreateComment(Guid blogPostId, [FromBody] CommentCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var blogPost = await _blogPostService.GetBlogPostAsync(blogPostId);
        if (blogPost is null) return NotFound("Blog post was not found.");
        
        if (blogPost.UserId == userId) return Forbid();
        
        var result = await _blogPostService.CreateCommentAsync(blogPostId, userId, request);
        if (result.Error is not null) return BadRequest(result.Error);
        
        return CreatedAtAction
            (
                nameof(GetBlogPost),
                new { id = blogPostId },
                result.Response
            );
    }

    [HttpGet("{blogPostId:guid}/comments")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetComments(Guid blogPostId)
    {
        var result = await _blogPostService.GetCommentsAsync(blogPostId);
        if (result.Error is not null) return NotFound(result.Error);
        
        return Ok(result.Response);
    }

    private Guid GetCurrentUserId()
    {
        var userIdValue = HttpContext.User.Claims
            .FirstOrDefault(c =>
                c.Type is ClaimTypes.NameIdentifier or JwtRegisteredClaimNames.Sub);
        
        return Guid.TryParse(userIdValue?.Value, out var userId)
            ? userId
            : Guid.Empty;
    }
}
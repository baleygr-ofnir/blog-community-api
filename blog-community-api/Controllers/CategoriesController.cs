using AutoMapper;
using blog_community_api.Contracts.BlogPosts;
using blog_community_api.Core.Interfaces;
using blog_community_api.Core.Services;
using blog_community_api.Data.Entities;
using blog_community_api.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blog_community_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;
    
    public CategoriesController(IService<Category> categoryService)
    {
        _categoryService = categoryService as CategoryService ?? throw new Exception("CategoryService is unavailable.");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
    {
        var response = await _categoryService.GetCategoriesAsync();
        if (!response.Any()) return NotFound();
        
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryResponse>> GetCategory([FromRoute] Guid id)
    {
        var response = await _categoryService.GetCategoryAsync(id);
        if (response == null) return NotFound();
        
        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CategoryRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _categoryService.CreateAsync(request);
        
        return CreatedAtAction
            (
                nameof(GetCategory),
               new { id = response.Id },
                response
            );
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] CategoryRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _categoryService.UpdateAsync(id, request);
        if (!updated) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted) return NotFound();
        
        return NoContent();
    }
}
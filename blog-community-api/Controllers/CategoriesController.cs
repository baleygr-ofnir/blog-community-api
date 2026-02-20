using AutoMapper;
using blog_community_api.Contracts.BlogPosts;
using blog_community_api.Data.Entities;
using blog_community_api.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blog_community_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(IRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
        {
            var categories = await _categoryRepository.AllAsync();
            if (!categories.Any()) return NotFound();
            
            var responses = _mapper.Map<List<CategoryResponse>>(categories);

            return Ok(responses);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponse>> GetCategory(Guid id)
        {
            var category = await _categoryRepository.GetAsync(id);
            if (category == null) return NotFound();
            
            var categoryResponse = _mapper.Map<CategoryResponse>(category);
            
            return Ok(categoryResponse);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var newCategory = _mapper.Map<Category>(request);
            newCategory.Id = Guid.NewGuid();
            
            await _categoryRepository.AddAsync(newCategory);
            await _categoryRepository.SaveChangesAsync();
            
            var response = _mapper.Map<CategoryResponse>(newCategory);
            
            return CreatedAtAction
                (
                    nameof(GetCategory),
                   new { id = response.Id },
                    response
                );
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var category = await _categoryRepository.GetAsync(id);
            if (category is null) return NotFound();

            _mapper.Map(request, category);
            
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var wasDeleted = await _categoryRepository.Delete(id);
            if (!wasDeleted) return NotFound();
            
            await _categoryRepository.SaveChangesAsync();
            
            return NoContent();
        }
    }
}

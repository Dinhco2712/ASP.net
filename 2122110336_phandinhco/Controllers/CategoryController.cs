using _2122110336_phandinhco.Data;
using _2122110336_phandinhco.Dto;
using _2122110336_phandinhco.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2122110336_phandinhco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            return await _context.categories.ToListAsync();
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var category = await _context.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST api/<CategoryController>
        //[Authorize(Policy = "AdminPolicy")] // Chỉ admin mới được tạo
        [HttpPost]
        public IActionResult Post([FromBody] CategoryDTO dto)
        {
            var category = new Category
            {
                name = dto.Name,
                avatar = dto.Avatar,
                craeteAt = DateTime.Now,
                updateAt = DateTime.Now
            };

            _context.categories.Add(category);
            _context.SaveChanges();

            return Ok(new { message = "Category created", category });
        }
        // PUT api/<CategoryController>/5
        //[Authorize(Policy = "AdminPolicy")] // Chỉ admin mới được tạo
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CategoryDTO dto)
        {
            var category = _context.categories.FirstOrDefault(c => c.id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            category.name = dto.Name;
            category.avatar = dto.Avatar;
            category.updateAt = DateTime.Now;

            _context.categories.Update(category);
            _context.SaveChanges();

            return Ok(new { message = "Category updated", category });
        }

        // DELETE api/<CategoryController>/5
        //[Authorize(Policy = "AdminPolicy")] // Chỉ admin mới được tạo
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

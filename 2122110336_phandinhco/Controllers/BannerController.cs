using _2122110336_phandinhco.Data;
using _2122110336_phandinhco.Dto;
using _2122110336_phandinhco.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2122110336_phandinhco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BannerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<BannerController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> Get()
        {
            return await _context.Banners.Include(b => b.Category).AsNoTracking().ToListAsync();
        }

        // GET api/<BannerController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Banner>> Get(int id)
        {
            var banner = await _context.Banners.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
            if (banner == null)
            {
                return NotFound(new { message = $"Banner with ID {id} not found." });
            }
            return banner;
        }

        // POST api/<BannerController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BannerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryExists = await _context.categories.AnyAsync(c => c.id == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = $"Category with ID {dto.CategoryId} does not exist." });
            }

            var banner = new Banner
            {
                Title = dto.Title,
                ImageUrl = dto.ImageUrl,
                Link = dto.Link,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = banner.Id }, new { message = "Banner created", banner });
        }

        // PUT api/<BannerController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BannerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound(new { message = $"Banner with ID {id} not found." });
            }

            var categoryExists = await _context.categories.AnyAsync(c => c.id == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = $"Category with ID {dto.CategoryId} does not exist." });
            }

            banner.Title = dto.Title;
            banner.ImageUrl = dto.ImageUrl;
            banner.Link = dto.Link;
            banner.CategoryId = dto.CategoryId;
            banner.UpdatedAt = DateTime.Now;

            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Banner updated", banner });
        }

        // DELETE api/<BannerController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound(new { message = $"Banner with ID {id} not found." });
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
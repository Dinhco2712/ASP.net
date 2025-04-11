using _2122110336_phandinhco.Data;
using _2122110336_phandinhco.Dto;
using _2122110336_phandinhco.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2122110336_phandinhco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    Id = p.id,
                    Name = p.name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.name
                }).ToListAsync();

            return Ok(products);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.id == id)
                .Select(p => new ProductDto
                {
                    Id = p.id,
                    Name = p.name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.name
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm" });
            }

            return Ok(product);
        }


        // POST api/Product
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new { message = "Invalid product data" });
            }

            product.createAt = DateTime.Now;
            product.updateAt = DateTime.Now;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.id }, product);
        }

        // PUT api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            if (id != product.id)
            {
                return BadRequest(new { message = "Product ID mismatch" });
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            existingProduct.name = product.name;
            existingProduct.avatar = product.avatar;
            existingProduct.price = product.price;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.updateAt = DateTime.Now;

            _context.Entry(existingProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.id == id))
                {
                    return NotFound(new { message = "Product not found during update" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

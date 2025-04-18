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
                    CategoryName = p.Category.name,
                    avatar =p.avatar,
                    Price=p.price,
                    quantity=p.quantity,
                    Description=  p.description,
                    createAt= p.createAt,
                    updateAt = p.updateAt
                }).ToListAsync();

            return Ok(products);
        }
        // Upload hình ảnh cho sản phẩm
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateProductImage(int id, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.id == id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Save file into wwwroot/images
            var fileName = Path.GetFileName(image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            Directory.CreateDirectory(uploadPath); // Create directory if it doesn't exist

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Update image path in the database
            product.avatar = "/images/" + fileName;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            // Return DTO
            var productDto = new ProductDto
            {
                Id = product.id,
                Name = product. name,
                avatar = product.avatar
            };

            return Ok(productDto);
        }
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || string.IsNullOrEmpty(product.avatar))
            {
                return NotFound("Product or image not found");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.avatar.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image file not found");
            }

            var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = "image/" + Path.GetExtension(filePath).Trim('.'); // ví dụ: image/jpg, image/png

            return File(imageBytes, contentType);
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
        // POST api/<ProductController>
        [HttpPost]
       
        public IActionResult Post([FromBody] Product pro)
        {
            var category = _context.categories.FirstOrDefault(c => c.id == pro.CategoryId);
            if (category == null)
            {
                return BadRequest("Invalid category ID");
            }

            var product = new Product
            {
                name = pro.name,
                avatar = pro.avatar,
                price = pro.price,
                CategoryId = pro.CategoryId,
                description = pro.description,
                quantity=pro.quantity,
                createAt = DateTime.Now,
                updateAt = DateTime.Now
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(new { message = "Product created", product });
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
            existingProduct.description = product.description;
            existingProduct.quantity = product.quantity;
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

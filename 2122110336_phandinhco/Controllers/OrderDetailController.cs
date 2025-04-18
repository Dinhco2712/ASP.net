using _2122110336_phandinhco.Data;
using _2122110336_phandinhco.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _2122110336_phandinhco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> Get()
        {
            return await _context.OrderDetails.Include(od => od.Product).Include(od => od.Order).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> Get(int id)
        {
            var orderDetail = await _context.OrderDetails.Include(od => od.Product).Include(od => od.Order).FirstOrDefaultAsync(od => od.Id == id);
            if (orderDetail == null) return NotFound();
            return orderDetail;
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
            return Ok(new { message = "OrderDetail created", orderDetail });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrderDetail orderDetail)
        {
            var existingOrderDetail = _context.OrderDetails.FirstOrDefault(od => od.Id == id);
            if (existingOrderDetail == null) return NotFound(new { message = "OrderDetail not found" });

            existingOrderDetail.ProductId = orderDetail.ProductId;
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.UnitPrice = orderDetail.UnitPrice;

            _context.OrderDetails.Update(existingOrderDetail);
            _context.SaveChanges();
            return Ok(new { message = "OrderDetail updated", existingOrderDetail });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return NotFound();

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

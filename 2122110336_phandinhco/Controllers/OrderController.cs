using _2122110336_phandinhco.Data;
using _2122110336_phandinhco.Dto;
using _2122110336_phandinhco.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Đảm bảo có namespace này cho Select và ToList

namespace _2122110336_phandinhco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return order;
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                UserId = orderDto.UserId,
                OrderDate = orderDto.OrderDate,
                TotalAmount = orderDto.TotalAmount,
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

            _context.Orders.Add(order);
            try
            {
                _context.SaveChanges();
                return Ok(new { message = "Order created", order });
            }
            catch (DbUpdateException ex)
            {
                // Log lỗi chi tiết (có thể ghi vào file log hoặc hệ thống giám sát)
                Console.WriteLine($"Error creating order: {ex}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "Failed to create order. Please check server logs.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrderDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrder = _context.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.Id == id);
            if (existingOrder == null) return NotFound(new { message = "Order not found" });

            existingOrder.UserId = orderDto.UserId;
            existingOrder.OrderDate = orderDto.OrderDate;
            existingOrder.TotalAmount = orderDto.TotalAmount;

            // Cập nhật OrderDetails (cần xem xét kỹ logic này)
            existingOrder.OrderDetails.Clear();
            existingOrder.OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
            {
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice
            }).ToList();

            _context.Orders.Update(existingOrder);
            try
            {
                _context.SaveChanges();
                return Ok(new { message = "Order updated", existingOrder });
            }
            catch (DbUpdateException ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Error updating order {id}: {ex}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "Failed to update order. Please check server logs.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
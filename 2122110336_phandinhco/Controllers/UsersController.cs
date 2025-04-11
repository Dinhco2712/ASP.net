using Microsoft.AspNetCore.Mvc;
using _2122110336_phandinhco.Model;
using _2122110336_phandinhco.Data;
using Microsoft.EntityFrameworkCore;
using _2122110336_phandinhco.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace _2122110336_phandinhco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDTO
                {
                    Id = u.id,
                    Name = u.name,
                    Email = u.email,
                    Role = u.role,
                    CreateAt = u.createAt,
                    UpdateAt = u.updateAt
                })
                .ToListAsync();

            return Ok(users);
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            user.password = HashPassword(user.password); // mã hoá
            user.createAt = DateTime.Now;
            user.updateAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }


        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User user)
        {
            if (id != user.id)
            {
                return BadRequest(new { message = "User ID mismatch" });
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            existingUser.name = user.name;
            existingUser.email = user.email;
            existingUser.password = user.password;
            existingUser.role = user.role;
            existingUser.updateAt = DateTime.Now;

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private string GenerateRandomToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var hashedPassword = HashPassword(loginDto.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == loginDto.Email && u.password == hashedPassword);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = GenerateRandomToken();

            return Ok(new
            {
                token,
                user = new
                {
                    user.id,
                    user.name,
                    user.email,
                    user.role
                }
            });
        }


    }

}

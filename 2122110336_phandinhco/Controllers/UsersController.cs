using _2122110336_phandinhco.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity.Data;
using _2122110336_phandinhco.Data;
using _2122110336_phandinhco.Model;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthController(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] _2122110336_phandinhco.Request.RegisterRequest request)
    {
        // Kiểm tra input
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        // Kiểm tra email đã tồn tại
        var existingUser = _context.Users.FirstOrDefault(u => u.email == request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already exists." });
        }

        // Mã hóa mật khẩu
        var user = new User
        {
            name = request.Name,
            email = request.Email
        };

        var passwordHasher = new PasswordHasher<User>();
        user.password = passwordHasher.HashPassword(user, request.Password);

        // Gán UserRoles
        user.UserRoles = request.Roles.Select(roleId => new UserRoles
        {
            RoleId = roleId,
            User = user
        }).ToList();

        _context.Users.Add(user);
        _context.SaveChanges();

        var token = GenerateJwtToken(user.email);

        return Ok(new
        {
            message = "User registered successfully",
            token = token
        });
    }



    [HttpPost("login")]
    public IActionResult Login([FromBody] UserDTO user)
    {
        // Kiểm tra input đơn giản
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        // Kiểm tra email và mật khẩu
        var existingUser = _context.Users.FirstOrDefault(u => u.email == user.Email);
        if (existingUser == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var passwordHasher = new PasswordHasher<UserDTO>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, existingUser.password, user.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = GenerateJwtToken(existingUser.email);

        return Ok(new
        {
            message = "Login successful",
            token = token
        });
    }

    private string GenerateJwtToken(string email)
    {
        var user = _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.email == email);

        if (user == null)
            throw new Exception("User not found.");

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.name)
    };

        // Thêm các role vào claims
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
        }

        var jwtKey = _configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "phandinhco",
            audience: "your-audience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    [HttpGet("user/{id}")]
    public IActionResult GetUser(int id)
    {
        // Tìm user theo ID
        var user = _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.id == id);

        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        // Trả về thông tin user
        return Ok(new
        {
            id = user.id,
            name = user.name,
            email = user.email,
            roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
            createAt = user.createAt,
            updateAt = user.updateAt
        });
    }
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        // Lấy danh sách tất cả người dùng
        var users = _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(user => new
            {
                id = user.id,
                name = user.name,
                email = user.email,
                roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                createAt = user.createAt,
                updateAt = user.updateAt
            })
            .ToList();

        return Ok(users);
    }


}
using Inventory_management_System.Dto.LoginDto;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventory_management_System.Controllers.inventroy
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly InventoryDBContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(InventoryDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
        [HttpPost("login")]
        public async Task<IActionResult> login(LoginRequest log)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Username == log.Username &&
                u.Password == log.Password);

            if (user == null) {
                return Unauthorized("Invalid credentials");
            }
            var token = GenerateJwtToken(user);
            return Ok(token);

        }



        private string GenerateJwtToken(BaseUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.Name),
                new Claim(ClaimTypes.Role, user.Role)

            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }


    }
}

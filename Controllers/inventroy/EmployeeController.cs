using Inventory_management_System.Dto.Employee;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

namespace Inventory_management_System.Controllers.inventroy
{
    [ApiController]
    [Route("[Controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly InventoryDBContext _context;
        private readonly IPasswordHasher<BaseUser> _passwordHasher;
        const string cachekey = "All_User";
        public EmployeeController(InventoryDBContext context, IMemoryCache cache, IPasswordHasher<BaseUser> passwordHasher)
        {
            _context = context;
            _cache = cache;
            _passwordHasher = passwordHasher;
        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!_cache.TryGetValue(cachekey, out List<Employee>? Emp))
                {
                    Emp = await _context.Employees.Include(s => s.BaseUser).ToListAsync();
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSize(1);
                    _cache.Set(cachekey, Emp, option);
                }
                return Ok(Emp);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GetById
        //[Authorize]
        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var key = $"User_{id}";
                if (!_cache.TryGetValue(key, out Employee? emp))
                {
                    emp = await _context.Employees.Include(s => s.BaseUser).FirstOrDefaultAsync(s => s.BaseUserId == id);
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSize(1); ;
                    _cache.Set(key, emp, option);
                }
                return Ok(emp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // Delete
        [Authorize(Roles = "Manager")]
        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var emp = await _context.Employees.FirstOrDefaultAsync(s => s.Id == id);
                var BId = emp.BaseUserId;
                var baseuser = await _context.Users.FirstOrDefaultAsync(s => s.Id == BId);

                _context.Employees.Remove(emp);
                _context.Users.Remove(baseuser);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _cache.Remove(cachekey);
                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }

        //Update
        [Authorize]
        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateEmployee emp)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
                var Emp = await _context.Users.FirstOrDefaultAsync(x => x.Id == employee.BaseUserId);
                Emp.Name = emp.Name;

                var result = _passwordHasher.VerifyHashedPassword(Emp, Emp.Password, emp.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return Unauthorized();
                }
                Emp.Password = _passwordHasher.HashPassword(Emp, emp.Password);

                _context.Users.Attach(Emp);
                _context.Users.Attach(Emp).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _cache.Remove(cachekey);
                return Ok("Success");

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return NotFound();
            }


        }

        //POST

        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateEmployee emp)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var employee = new BaseUser
                {
                    Name = emp.Name,
                    Username = emp.Username,
                    Password = emp.Password,
                    Role = "Employee"

                };

                employee.Password = _passwordHasher.HashPassword(employee, emp.Password);
                await _context.Users.AddAsync(employee);
                await _context.SaveChangesAsync();

                var new_Employee = new Employee { BaseUserId = employee.Id };
                await _context.Employees.AddAsync(new_Employee);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _cache.Remove(cachekey);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }
    }
}

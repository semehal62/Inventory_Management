using Inventory_management_System.Dto.Employee;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_management_System.Controllers.inventroy
{
    [ApiController]
    [Route("[Controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly InventoryDBContext _context;
        const string cachekey = "All_Employee";
        public EmployeeController(InventoryDBContext context, IMemoryCache cache)
        {
            _context = context;

            _cache = cache;
        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            if (!_cache.TryGetValue(cachekey, out List<Employee>? Emp))
            {
                Emp = await _context.Employees.Include(s => s.BaseUser).ToListAsync();

                if (Emp == null)
                {
                    return NotFound("There is no Employee");
                }
                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSize(1);

                _cache.Set(cachekey, Emp, option);
            }
            return Ok(Emp);
        }

        // GetById
        [Authorize]
        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            var key = $"Employee{id}";
            if (!_cache.TryGetValue(key, out Employee? emp))
            {
                emp = await _context.Employees.Include(s => s.BaseUser).FirstOrDefaultAsync(s => s.BaseUserId == id);

                if (emp == null)
                {
                    return BadRequest("There is no such  an employee");

                }
                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(key, emp, option);
            }
            return Ok(emp);
        }
        // Delete
        [Authorize(Roles = "Manager")]
        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var emp = await _context.Employees.FirstOrDefaultAsync(s => s.id == id);
                var BId = emp.BaseUserId;
                var baseuser = await _context.Users.FirstOrDefaultAsync(s => s.Id == BId);

                _context.Employees.Remove(emp);
                _context.Users.Remove(baseuser);
                var result = await _context.SaveChangesAsync();


                _cache.Remove(cachekey);
                transaction.Commit();
                return Ok("Deleted");

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest();
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
                var employee = await _context.Employees.FirstOrDefaultAsync(x => x.id == id);
                var Emp = await _context.Users.FirstOrDefaultAsync(x => x.Id == employee.BaseUserId);
                Emp.Name = emp.Name;

                _context.Users.Attach(Emp);
                _context.Users.Attach(Emp).State = EntityState.Modified;

                var result = await _context.SaveChangesAsync();
                _cache.Remove(cachekey);

                transaction.Commit();
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
                await _context.Users.AddAsync(employee);

                var new_Employee = new Employee { BaseUserId = employee.Id };
                await _context.Employees.AddAsync(new_Employee);

                _cache.Remove(cachekey);
                transaction.Commit();
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

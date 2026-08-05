using Inventory_management_System.Dto.Employee;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_management_System.Controllers.inventroy
{
    [ApiController]
    [Route("[Controller]")]
    public class EmployeeController : ControllerBase
    {
        public readonly InventoryDBContext _context;
        public EmployeeController(InventoryDBContext context)
        {
            _context = context;
        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var Emp = await _context.Users.Where(s => s.Role == "Employee").ToListAsync();

            if (Emp == null)
            {
                return NotFound("There is no Employee");
            }

            return Ok(Emp);
        }

        // GetById
        [Authorize]
        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {

            var emp = await _context.Users.FirstOrDefaultAsync(s => s.Id == id && s.Role == "Employee");

            if (emp == null)
            {
                return BadRequest("There is no such  an employee");

            }

            return Ok(emp);
        }
        // Delete
        [Authorize(Roles = "Manager")]
        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _context.Users.FirstOrDefaultAsync(s => s.Id == id && s.Role == "Employee");
            if (emp == null)
            {
                return BadRequest();
            }
            _context.Remove(emp);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var R_employee = await _context.Employees.FirstOrDefaultAsync(s => s.BaseUserId == emp.Id);
                _context.Employees.Remove(R_employee);
                var res = await _context.SaveChangesAsync();
                if (res > 0)
                {
                    return Ok("Deleted");
                }
            }
            return BadRequest();

        }

        //Update
        [Authorize(Roles = "Employee")]
        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateEmployee emp)
        {
            var employee = await _context.Users.FirstOrDefaultAsync(x => x.Id == id && x.Role == "Employee");
            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = emp.Name;
            employee.Username = emp.Username;
            employee.Password = emp.Password;

            _context.Users.Attach(employee);
            _context.Users.Attach(employee).State = EntityState.Modified;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok("Success");
            }

            return NotFound();

        }

        //POST
        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateEmployee emp)
        {
            var employee = new BaseUser
            {
                Name = emp.Name,
                Username = emp.Username,
                Password = emp.Password,
                Role = emp.Role

            };

            await _context.Users.AddAsync(employee);
            var result = await _context.SaveChangesAsync();


            if (result > 0)
            {
                var new_emp = new Employee
                {
                    BaseUserId = employee.Id
                };

                await _context.Employees.AddAsync(new_emp);
                var res = await _context.SaveChangesAsync();

                if (res > 0)
                {
                    return Ok("Success");
                }
                return BadRequest();
            }

            return BadRequest(result);
        }
    }
}

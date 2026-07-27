using Inventory_management_System.Dto.Employee;
using Inventory_management_System.Models;
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
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var Emp = await _context.Employees.ToListAsync();

            if (Emp == null)
            {
                return NotFound("There is no Employee");
            }

            return Ok(Emp);
        }

        // GetById

        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {

            var emp = await _context.Employees.FirstOrDefaultAsync(s => s.Id == id);

            if (emp == null)
            {
                return BadRequest("There is no such  an employee");

            }

            return Ok(emp);
        }
        // Delete

        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _context.Employees.FirstOrDefaultAsync(s => s.Id == id);
            if (emp == null)
            {
                return BadRequest();
            }
            _context.Remove(emp);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("Deleted");
            }
            return BadRequest();

        }

        //Update

        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateEmployee emp)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = emp.Name;

            _context.Employees.Attach(employee);
            _context.Employees.Attach(employee).State = EntityState.Modified;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok("Success");
            }

            return NotFound();

        }

        //POST
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateEmployee emp)
        {
            var employee = new Employee
            {
                Name = emp.Name

            };

            await _context.Employees.AddAsync(employee);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("Success");
            }

            return BadRequest(result);
        }
    }
}

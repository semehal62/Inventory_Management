using Inventory_management_System.Dto.Employee;
using Inventory_management_System.Dto.Manager;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class ManagerController: ControllerBase
    {
        public readonly InventoryDBContext _context;
        public ManagerController(InventoryDBContext context)
        {
            _context = context;
        }

        // GETAll
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var manager = await _context.Managers.ToListAsync();

            if (manager == null)
            {
                return NotFound("There is no Manager");
            }

            return Ok(manager);
        }


        // GetById

        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {

            var man = await _context.Managers.FirstOrDefaultAsync(s => s.Id == id);

            if (man == null)
            {
                return BadRequest("There is no such manager");

            }

            return Ok(man);
        }

        // Delete

        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(s => s.Id == id);
            if (manager == null)
            {
                return BadRequest();
            }

            _context.Managers.Remove(manager);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("Deleted");
            }
            return BadRequest();

        }


        //Update

        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateManager man)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.Id == id);
            if (manager == null)
            {
                return NotFound();
            }

            manager.Name = man.Name;

            _context.Managers.Attach(manager);
            _context.Managers.Attach(manager).State = EntityState.Modified;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok("Updated");
            }

            return NotFound();

        }

        //POST
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateManager man)
        {
            var manager = new Manager
            {
                Name = man.Name

            };

            await _context.Managers.AddAsync(manager);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("Created");
            }

            return BadRequest(result);
        }


    }
}

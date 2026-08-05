using Inventory_management_System.Dto.Manager;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class ManagerController : ControllerBase
    {
        public readonly InventoryDBContext _context;
        public ManagerController(InventoryDBContext context)
        {
            _context = context;
        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var manager = await _context.Users.Where(s => s.Role == "Manager").ToListAsync();

            if (manager == null)
            {
                return NotFound("There is no Manager");
            }

            return Ok(manager);
        }


        // GetById
        [Authorize]
        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {

            var man = await _context.Users.FirstOrDefaultAsync(s => s.Id == id && s.Role == "Manager");

            if (man == null)
            {
                return BadRequest("There is no such manager");

            }

            return Ok(man);
        }

        // Delete
        [Authorize(Roles = "Manager")]
        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var manager = await _context.Users.FirstOrDefaultAsync(s => s.Id == id && s.Role == "Manager");
            if (manager == null)
            {
                return BadRequest();
            }

            _context.Users.Remove(manager);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var R_manager = await _context.Managers.FirstOrDefaultAsync(s => s.BaseUserId == manager.Id);
                _context.Managers.Remove(R_manager);
                 var res = await _context.SaveChangesAsync();
                if (res > 0)
                {

                    return Ok("Deleted");
                }
                return BadRequest();
            }
            return BadRequest();

        }


        //Update
        [Authorize(Roles = "Manager")]
        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateManager man)
        {
            var manager = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (manager == null)
            {
                return NotFound();
            }

            manager.Name = man.Name;
            manager.Username = man.Username;
            manager.Password = man.Password;

            _context.Users.Attach(manager);
            _context.Users.Attach(manager).State = EntityState.Modified;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok("Updated");
            }

            return NotFound();

        }

        //POST
        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateManager man)
        {
            var manager = new BaseUser
            {
                Name = man.Name,
                Username = man.Username,
                Password = man.Password,
                Role = man.Role

            };

            await _context.Users.AddAsync(manager);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var new_man = new Manager { BaseUserId = manager.Id };
                await _context.Managers.AddAsync(new_man);
                var res = await _context.SaveChangesAsync();
                if (res > 0)
                {
                    return Ok("Created");
                }
                return BadRequest();
            }

            return BadRequest(result);
        }


    }
}

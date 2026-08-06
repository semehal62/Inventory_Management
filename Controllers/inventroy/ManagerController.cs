using Inventory_management_System.Dto.Employee;
using Inventory_management_System.Dto.Manager;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly InventoryDBContext _context;
        private readonly IMemoryCache _cache;
        private const string cachekey = "All_Manager";
        public ManagerController(InventoryDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GETAll
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {

            if (!_cache.TryGetValue(cachekey, out List<Manager>? manager)){
                manager = await _context.Managers.ToListAsync();

                if (manager == null)
                {
                    return NotFound("There is no Manager");
                }

                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cachekey, manager, option);
            }

            return Ok(manager);
        }


        // GetById

        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            var key = $"Manager_{id}";

            if (!_cache.TryGetValue(key, out Manager ? man))
            {
                man =  await _context.Managers.FirstOrDefaultAsync(s => s.Id == id);

                if (man == null)
                {
                    return BadRequest("There is no such manager");

                }
                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
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
                _cache.Remove(cachekey);
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
                _cache.Remove(cachekey);
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
                _cache.Remove(cachekey);
                return Ok("Created");
            }

            return BadRequest(result);
        }


    }
}

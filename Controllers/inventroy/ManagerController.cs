using Inventory_management_System.Dto.Manager;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {

            if (!_cache.TryGetValue(cachekey, out List<Manager>? manager))
            {
                manager = await _context.Managers.Include(s => s.BaseUser).ToListAsync();

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
        [Authorize]
        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            var key = $"Manager_{id}";

            if (!_cache.TryGetValue(key, out Manager? man))
            {
                man = await _context.Managers.Include(s => s.BaseUser).FirstOrDefaultAsync(s => s.id == id);

                if (man == null)
                {
                    return BadRequest("There is no such manager");

                }
                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            }

            return Ok(man);
        }

        // Delete
        [Authorize(Roles = "Manager")]

        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var manager = await _context.Managers.FirstOrDefaultAsync(s => s.id == id);
                var Buser = await _context.Users.FirstOrDefaultAsync(s => s.Id == manager.BaseUserId);

                _context.Managers.Remove(manager);
                _context.Users.Remove(Buser);
                _cache.Remove(cachekey);

                transaction.Commit();
                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }


        //Update
        [Authorize(Roles = "Manager")]

        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateManager man)
        {
            var transaction = await  _context.Database.BeginTransactionAsync();
            try
            {
                var manager = await _context.Managers.FirstOrDefaultAsync(x => x.id == id);

                var Buser = await _context.Users.FirstOrDefaultAsync(s => s.Id == manager.BaseUserId);
                Buser.Name = man.Name;

                _context.Users.Attach(Buser);
                _cache.Remove(cachekey);

                transaction.Commit();
                return Ok("Updated");

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return NotFound(e.Message);

            }

        }

        //POST
        [Authorize(Roles = "Manager")]

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateManager man)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var Buser = new BaseUser
                {
                    Name = man.Name,
                    Username = man.Username,
                    Password = man.Password,
                    Role = "Manager"

                };

                await _context.Users.AddAsync(Buser);
                var manager = new Manager { BaseUserId = Buser.Id };

                await _context.Managers.AddAsync(manager);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _cache.Remove(cachekey);
                return Ok("Created");

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);

            }



        }


    }
}

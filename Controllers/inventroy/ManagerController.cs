using Inventory_management_System.Dto.Manager;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly InventoryDBContext _context;
        private readonly IMemoryCache _cache;
        private readonly IPasswordHasher<BaseUser> _PasswordHasher;
        private const string cachekey = "All_Manager";
        public ManagerController(InventoryDBContext context, IMemoryCache cache,IPasswordHasher<BaseUser> passwordHasher)
        {
            _context = context;
            _cache = cache;
            _PasswordHasher  = passwordHasher;
        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!_cache.TryGetValue(cachekey, out List<Manager>? manager))
                {
                    manager = await _context.Managers.Include(s => s.BaseUser).ToListAsync();
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    _cache.Set(cachekey, manager, option);
                }
                return Ok(manager);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        // GetById
        [Authorize]
        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var key = $"Manager_{id}";

                if (!_cache.TryGetValue(key, out Manager? man))
                {
                    man = await _context.Managers.Include(s => s.BaseUser).FirstOrDefaultAsync(s => s.id == id);
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    _cache.Set(cachekey,man, option);
                }

                return Ok(man);
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
                var manager = await _context.Managers.FirstOrDefaultAsync(s => s.id == id);
                var Buser = await _context.Users.FirstOrDefaultAsync(s => s.id == manager.BaseUserId);

                _context.Managers.Remove(manager);
                _context.Users.Remove(Buser);

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
        [Authorize(Roles = "Manager")]

        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateManager man)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var manager = await _context.Managers.FirstOrDefaultAsync(x => x.id == id);

                var Buser = await _context.Users.FirstOrDefaultAsync(s => s.id == manager.BaseUserId);
                Buser.Name = man.Name;

                _context.Users.Attach(Buser);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _cache.Remove(cachekey);
                return Ok("Updated");

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return NotFound(e.Message);

            }

        }

        //POST

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

                Buser.Password = _PasswordHasher.HashPassword(Buser, man.Password);
                await _context.Users.AddAsync(Buser);
                await _context.SaveChangesAsync();

                var manager = new Manager { BaseUserId  = Buser.id };
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

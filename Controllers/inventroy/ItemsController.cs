using Inventory_management_System.Dto.Item;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly InventoryDBContext _context;
        private readonly IMemoryCache _cache;
        private const string cachekey = "All_items";
        public ItemsController(InventoryDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }


        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!_cache.TryGetValue(cachekey, out List<Item>? item))
                {
                    item = await _context.Items.Include(s => s.Manager).ThenInclude(x =>x.BaseUser).ToListAsync();
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)).SetSize(1);

                    _cache.Set(cachekey, item, option);
                }

                return Ok(item);
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
                string key = $"item_{id}";

                if (!_cache.TryGetValue(key, out Item? item))
                {
                    item = await _context.Items.Include(s => s.Manager).ThenInclude(x => x.BaseUser).FirstOrDefaultAsync(s => s.Id == id);
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)).SetSize(1);
                    _cache.Set(key, item, option);
                }
                return Ok(item);
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
                var item = await _context.Items.FirstOrDefaultAsync(s => s.Id == id);
                _context.Items.Remove(item);

                await _context.SaveChangesAsync();
                transaction.Commit();
                _cache.Remove(cachekey);
                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        //PUT
        [Authorize(Roles = "Manager")]
        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateItem item)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var it = await _context.Items.FirstOrDefaultAsync(s => s.Id == id);

                it.Name = item.Name;
                it.Quantity = item.Quantity;
                it.ManagerId = item.ManagerId;
                it.Enter_date = DateTime.UtcNow;
                it.Price = item.Prices;

                _context.Items.Attach(it);
                _context.Items.Attach(it).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                transaction.Commit();
                _cache.Remove(cachekey);
                return Ok("success");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);

            }

        }



        //POST
        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateItem item)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var baseuserIdstring  = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(!int.TryParse(baseuserIdstring, out int baseuserId))
                {
                    return NotFound();
                }
                var managerId = await _context.Managers.FirstOrDefaultAsync(s => s.BaseUserId == baseuserId);
                if(managerId == null)
                {
                    return Unauthorized();
                }

                var it = new Item
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    ManagerId = managerId.Id,
                    Price = item.Prices
                };

                await _context.Items.AddAsync(it);

                await _context.SaveChangesAsync();
                transaction.Commit();
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

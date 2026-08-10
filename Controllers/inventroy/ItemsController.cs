using Inventory_management_System.Dto.Item;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

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

            if (!_cache.TryGetValue(cachekey, out  List<Item> ? item))
            {
                 item = await _context.Items.ToListAsync();

                if (item == null)
                {
                    return NotFound("There is no Item");
                }
                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)).SetSize(1);

                _cache.Set(cachekey, item, option);
            }

            return Ok(item);
        }

        // GetById
        [Authorize]
        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            string key = $"item_{id}";

            if (!_cache.TryGetValue(key, out Item? item))
            {
                item = await _context.Items.FirstOrDefaultAsync(s => s.Id == id);

                if (item == null)
                {
                    return BadRequest("There is no such Item");

                }

                _cache.Set(key, item,TimeSpan.FromMinutes(10));
            }
            return Ok(item);
        }

        // Delete

        
        [Authorize(Roles = "Manager")]
        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(s => s.Id == id);
            if (item == null)
            {
                return BadRequest("Id not found");
            }
            _context.Items.Remove(item);
            var result = await _context.SaveChangesAsync();

            if (result < 0)
            {
                return BadRequest("Can't delete");
            }
            _cache.Remove(cachekey);

            return Ok("Deleted");
        }

        //PUt
        [Authorize(Roles = "Manager")]
        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateItem item)
        {
            var it = await _context.Items.FirstOrDefaultAsync(s => s.Id == id);
            if (it == null)
            {
                return BadRequest();
            }

            it.Name = item.Name;
            it.Quantity = item.Quantity;
            it.MangerId = item.ManagerId;
            it.Enter_date = DateTime.UtcNow;
            it.Price = item.Prices;

            _context.Items.Attach(it);
            _context.Items.Attach(it).State = EntityState.Modified;

            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return BadRequest();
            }

            _cache.Remove(cachekey);

            return Ok("success");

        }



        //POST
        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateItem item)
        {
            var it = new Item
            {
                Name = item.Name,
                Quantity = item.Quantity,
                MangerId = item.ManagerId,
                Price = item.Prices
            };

            await _context.Items.AddAsync(it);
            var result = await _context.SaveChangesAsync();
            if (result <= 0)
            {
                return BadRequest();

            }
            _cache.Remove(cachekey);

            return Ok("Created");

        }
    }
}

using Inventory_management_System.Dto.Sales;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class SalesController : ControllerBase
    {
        public readonly InventoryDBContext _context;
        private readonly IMemoryCache _cache;
        const string cachekey = "All_Sales";
        public SalesController(InventoryDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GETAll
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            if (!_cache.TryGetValue(cachekey, out List<Sale>? sale))
            {
                sale = await _context.Sales.ToListAsync();


                if (sale == null)
                {
                    return NotFound("There is no Sales");
                }

                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cachekey,sale, option);
            }
            return Ok(sale);
        }

        // GetById

        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            var key = $"Sales{id}";

            if (!_cache.TryGetValue(key, out Sale ? sale))
            {

                sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id);

                if (sale == null)
                {
                    return BadRequest("There is no such employee");

                }
                var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cachekey, sale, option);

            }
            return Ok(sale);
        }

        // Delete

        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
            {
                return BadRequest();
            }
            _context.Sales.Remove(sale);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _cache.Remove(cachekey);
                return Ok("Deleted");
            }
            return BadRequest();

        }

        //PUT
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Updated(int id, CreateSale sal)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(d => d.Id == id);
            if (sale == null)
            {
                return BadRequest();
            }
            sale.EmployeeId = sal.EmployeeId;
            sale.ItemsId = sal.ItemsId;
            sale.Quantity_Sold = sal.Quantity_Sold;
            sale.Total_prices = sal.Total_prices;
            sale.Sold_date = DateTime.UtcNow;

            _context.Sales.Attach(sale);
            _context.Sales.Attach(sale).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _cache.Remove(cachekey);
                return Ok("suncess");
            }
            return BadRequest();
        }

        //POST
        [HttpPost("Create")]

        public async Task<IActionResult> Create(CreateSale sale)
        {
            var sold = new Sale
            {
                EmployeeId = sale.EmployeeId,
                ItemsId = sale.ItemsId,
                Quantity_Sold = sale.Quantity_Sold,
                Total_prices = sale.Total_prices

            };


            await _context.Sales.AddAsync(sold);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _cache.Remove(cachekey);
                return Ok("Create");
            }
            return NotFound();
        }
    }


}

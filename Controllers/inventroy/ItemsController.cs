using Inventory_management_System.Dto.Item;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class ItemsController : ControllerBase
    {
        public readonly InventoryDBContext _context;

        public ItemsController(InventoryDBContext context)
        {
            _context = context;
        }


        // GETAll
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var item = await _context.Items.ToListAsync();

            if (item == null)
            {
                return NotFound("There is no Item");
            }

            return Ok(item);
        }

        // GetById

        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {

            var item = await _context.Items.FirstOrDefaultAsync(s => s.Id == id);

            if (item == null)
            {
                return BadRequest("There is no such Item");

            }

            return Ok(item);
        }

        // Delete

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

            if (result > 0)
            {
                return Ok("Deleted");
            }
            return BadRequest("Can't delete");

        }

        //PUt
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

            return Ok("success");

        }



        //POST
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
            if (result > 0)
            {
                return Ok("Created");

            }
            return BadRequest();
        }
    }
}

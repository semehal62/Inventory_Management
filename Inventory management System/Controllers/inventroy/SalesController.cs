using Inventory_management_System.Dto.Sales;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class SalesController : ControllerBase
    {
        public readonly InventoryDBContext _context;
        public SalesController(InventoryDBContext context)
        {
            _context = context;
        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var sale = await _context.Sales.Include(s=> s.Users).ToListAsync();

            if (sale == null)
            {
                return NotFound("There is no Sales");
            }

            return Ok(sale);
        }

        // GetByIdByEmpId
        [Authorize]
        [HttpGet("GetByEmpId/{id}")]

        public async Task<IActionResult> GetById(int id)
        {

            var sale = await _context.Sales.Include(s => s.Users).Include(s => s.Items).Where(s => s.BaseUserId == id).ToListAsync();

            if (sale == null)
            {
                return BadRequest("There is no such employee");

            }

            return Ok(sale);
        }

        // Delete
        [Authorize(Roles = "Manager")]
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
                return Ok("Deleted");
            }
            return BadRequest();

        }

        //PUT
        [Authorize]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Updated(int id, CreateSale sal)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(d => d.Id == id);
            if (sale == null)
            {
                return BadRequest();
            }
            sale.BaseUserId = sal.BaseUserId;
            sale.ItemsId = sal.ItemsId;
            sale.Quantity_Sold = sal.Quantity_Sold;
            sale.Total_prices = sal.Total_prices;
            sale.Sold_date = DateTime.UtcNow;

            _context.Sales.Attach(sale);
            _context.Sales.Attach(sale).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("suncess");
            }
            return BadRequest();
        }

        //POST
        [Authorize]
        [HttpPost("Create")]


        public async Task<IActionResult> Create(CreateSale sale)
        {
            var sold = new Sale
            {
                BaseUserId = sale.BaseUserId,
                ItemsId = sale.ItemsId,
                Quantity_Sold = sale.Quantity_Sold,
                Total_prices = sale.Total_prices

            };


            await _context.Sales.AddAsync(sold);
            var result = await _context.SaveChangesAsync();
       

            if (result > 0)
            {
                return Ok("Create");
            }
            return NotFound();
        }
    }


}

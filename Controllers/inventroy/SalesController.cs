using Inventory_management_System.Dto.Sales;
using Inventory_management_System.Models;
using Inventory_management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class SalesController : ControllerBase
    {
        public readonly InventoryDBContext _context;
        private readonly IMemoryCache _cache;
        private readonly IAIServices _aiservices;
        const string cachekey = "All_Sales";
        public SalesController(InventoryDBContext context, IMemoryCache cache, IAIServices aisevices)
        {
            _context = context;
            _cache = cache;
            _aiservices = aisevices;
        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!_cache.TryGetValue(cachekey, out List<Sale>? sale))
                {
                    sale = await _context.Sales.Include(s => s.BaseUser).ToListAsync();
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSize(1);
                    _cache.Set(cachekey, sale, option);
                }
                return Ok(sale);
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
                var key = $"Sales{id}";

                if (!_cache.TryGetValue(key, out Sale? sale))
                {
                    sale = await _context.Sales.Include(s => s.BaseUser).FirstOrDefaultAsync(s => s.Id == id);
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSize(1);
                    _cache.Set(cachekey, sale, option);
                }
                return Ok(sale);
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
                var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id);
                _context.Sales.Remove(sale);
                _cache.Remove(cachekey);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }

        //PUT

        [Authorize]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Updated(int id, CreateSale sal)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var baseuserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(baseuserId, out int baseuser))
                {
                    return Unauthorized();
                }
                var sale = await _context.Sales.FirstOrDefaultAsync(d => d.Id == id);
                sale.BaseUserId = baseuser;
                sale.ItemsId = sal.ItemsId;
                sale.Quantity_Sold = sal.Quantity_Sold;
                sale.Total_prices = sal.Total_prices;
                sale.Sold_date = DateTime.UtcNow;

                var airesult = await _aiservices.AnalyzeSaleAsync(sale);

                var audit = await _context.Audit_logs.FirstOrDefaultAsync(s => s.SoldId == id);

                audit.AI_Status = airesult.Status;
                audit.Anomalies_Detected = airesult.Anomalies_Detected;
                audit.Explanation = airesult.Explanation;

                _context.Audit_logs.Attach(audit);
                _context.Sales.Attach(sale);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
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
        [Authorize]
        [HttpPost("Create")]

        public async Task<IActionResult> Create(CreateSale sale)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var baseUseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(!int.TryParse(baseUseId,out int baseuId))
                {
                    return NotFound();
                }
                var sold = new Sale
                {
                    BaseUserId = baseuId,
                    Sold_date = DateTime.UtcNow,
                    ItemsId = sale.ItemsId,
                    Quantity_Sold = sale.Quantity_Sold,
                    Total_prices = sale.Total_prices

                };

                await _context.Sales.AddAsync(sold);
                await _context.SaveChangesAsync();

                var aiResult = await _aiservices.AnalyzeSaleAsync(sold);

                var auditLog = new Audit_Log
                {
                    SoldId = sold.Id,
                    AI_Status = aiResult.Status,
                    Anomalies_Detected = aiResult.Anomalies_Detected,
                    Explanation = aiResult.Explanation
                };

                var item = await _context.Items.FirstOrDefaultAsync(s => s.Id == sale.ItemsId);
                if(aiResult.Status.ToString() == "Approved")
                {
                    item.Quantity -= sale.Quantity_Sold;
                }

                await _context.Audit_logs.AddAsync(auditLog);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _cache.Remove(cachekey);

                return Ok(new
                {
                    Message = "Sale created and audited successfully",
                    SaleId = sold.Id,
                    Audit = aiResult
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }
    }


}

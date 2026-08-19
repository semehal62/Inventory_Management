using Inventory_management_System.Dto.Audit_log;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class Audits_logController : ControllerBase
    {
        private readonly InventoryDBContext _context;
        private readonly IMemoryCache _cache;

        const string cachekey = "All_Audits";

        public Audits_logController(InventoryDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;

        }

        // GETAll
        [Authorize]
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            //AudtVeiwDto ? auditData = null;

            try
            {
                if (!_cache.TryGetValue(cachekey, out List<AudtViewDto>? auditData))
                {
                    var Audit = await _context.Audit_logs.ToListAsync();
                    auditData = Audit.Select(p => new AudtViewDto
                    {
                        Id = p.Id,
                        Sold = p.Sold,
                        SoldId = p.SoldId,
                        AI_Status = p.AI_Status.ToString(),
                        Anomalies_Detedced = p.Anomalies_Detected,
                        Explanation = p.Explanation

                    }).ToList();

                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSize(1); 
                    _cache.Set(cachekey, auditData, option);

                }
                return Ok(auditData);
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
                var key = $"Audit{id}";
                if (!_cache.TryGetValue(key, out var audit))
                {
                    audit = await _context.Audit_logs.FirstOrDefaultAsync(s => s.Id == id);
                    var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetSize(1); 
                    _cache.Set(key, audit, option);
                }
                return Ok(audit);
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
                var audit = await _context.Audit_logs.FirstOrDefaultAsync(s => s.Id == id);
                _context.Audit_logs.Remove(audit);

                await _context.SaveChangesAsync();
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

        //PUT
        [Authorize(Roles = "Manager")]
        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateAudit_log aud)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var audit = await _context.Audit_logs.FirstOrDefaultAsync(s => s.Id == id);
                audit.AI_Status = aud.AI_Status;

                audit.Anomalies_Detected = aud.Anomalies_Detedced;
                audit.SoldId = aud.SoldId;
                audit.Explanation = aud.Explanation;

                _context.Audit_logs.Attach(audit);
                _context.Audit_logs.Attach(audit).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _cache.Remove(cachekey);
                return Ok("updated");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return NotFound(ex.Message);
            }

        }

        //
        ////POST
        //[Authorize(Roles = "Manager")]
        //[HttpPost("Create")]

        //public async Task<IActionResult> Create(CreateAudit_log aud)
        //{

        //    var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {

        //    var existingSale = await _context.Audit_logs.FirstOrDefaultAsync(s => s.SoldId == aud.SoldId);
        //    if (existingSale != null)
        //    {
        //        // update
        //        existingSale.AI_Status = aud.AI_Status;
        //        existingSale.Anomalies_Detected = aud.Anomalies_Detedced;

        //        _context.Audit_logs.Attach(existingSale);

        //        await _context.SaveChangesAsync();
        //        return Ok("Updated");

        //    }
        //    var Audit = new Audit_Log
        //    {
        //        AI_Status = aud.AI_Status,
        //        Anomalies_Detected = aud.Anomalies_Detedced,
        //        SoldId = aud.SoldId,
        //        Explanation = aud.Explanation
        //    };

        //    await _context.Audit_logs.AddAsync(Audit);
        //    var result = await _context.SaveChangesAsync();

        //    if (result > 0)
        //    {
        //        _cache.Remove(cachekey);
        //        return Ok("Created");
        //    }

        //    return BadRequest();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

    }
}

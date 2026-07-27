using Inventory_management_System.Dto.Audit_log;
using Inventory_management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_management_System.Controllers.inventroy
{

    [ApiController]
    [Route("[Controller]")]
    public class Audits_logController : ControllerBase
    {
        public readonly InventoryDBContext _context;

        public Audits_logController(InventoryDBContext context)
        {
            _context = context;

        }

        // GETAll
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var Audit = await _context.Audit_logs.ToListAsync();


            if (Audit == null)
            {
                return NotFound("There is no Audit");
            }
            var auditData = Audit.Select(p=> new AudtViewDto
            {
                Id = p.Id,
                Sold = p.Sold,
                SoldId = p.SoldId,
                AI_Status = p.AI_Status.ToString(),
                Anomalies_Detedced = p.Anomalies_Detedced

            }).ToList();


            return Ok(auditData);
        }


        // GetById

        [HttpGet("GetById/{id}")]

        public async Task<IActionResult> GetById(int id)
        {

            var audit = await _context.Audit_logs.FirstOrDefaultAsync(s => s.Id == id);

            if (audit == null)
            {
                return BadRequest("There is no such an Audit");

            }

            return Ok(audit);
        }

        // Delete

        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var audit = await _context.Audit_logs.FirstOrDefaultAsync(s => s.Id == id);
            if (audit == null)
            {
                return BadRequest();
            }
            _context.Audit_logs.Remove(audit);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("Deleted");
            }
            return BadRequest();

        }

        //PUT
        [HttpPut("Update/{id}")]

        public async Task<IActionResult> Update(int id, CreateAudit_log aud)
        {
            var audit = await _context.Audit_logs.FirstOrDefaultAsync(s => s.Id == id);
            if (audit == null)
            {
                return NotFound();
            }
            audit.AI_Status = aud.AI_Status;

            audit.Anomalies_Detedced = aud.Anomalies_Detedced;
            audit.SoldId = aud.SoldId;

            _context.Audit_logs.Attach(audit);
            _context.Audit_logs.Attach(audit).State = EntityState.Modified;

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("updated");
            }
            return NotFound();

        }


        //POST
        [HttpPost("Create")]

        public async Task<IActionResult> Create(CreateAudit_log aud)
        {


            var existingSale = await _context.Audit_logs.FirstOrDefaultAsync(s => s.SoldId == aud.SoldId);
            if (existingSale != null)
            {
                // update
                existingSale.AI_Status = aud.AI_Status;
                existingSale.Anomalies_Detedced = aud.Anomalies_Detedced;

                 _context.Audit_logs.Attach(existingSale);

                await _context.SaveChangesAsync();
                return Ok("Updated");

            }
            var Audit = new Audit_Log
            {
                AI_Status = aud.AI_Status,
                Anomalies_Detedced = aud.Anomalies_Detedced,
                SoldId = aud.SoldId
            };

            await _context.Audit_logs.AddAsync(Audit);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok("Created");
            }

            return BadRequest();
        }

    }
}

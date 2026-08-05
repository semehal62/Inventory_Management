using Inventory_management_System.Models;
namespace Inventory_management_System.Dto.Audit_log
{
    public class AudtViewDto
    {
        public int Id { get; set; }
        public Sale ? Sold { get; set; }
        public int SoldId { get; set; }
        public required string AI_Status { get; set; }
        public required String Anomalies_Detedced { get; set; }
 
    }
}

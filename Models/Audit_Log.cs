using System.ComponentModel.DataAnnotations.Schema;
namespace Inventory_management_System.Models
{
    public class Audit_Log
    {
        public int Id { get; set; }
        public Sale? Sold { get; set; }
        public int SoldId { get; set; }
        public required AI_Status AI_Status { get; set; }
        public required String Anomalies_Detected { get; set; }
        public required string Explanation { get; set; }
    }
}

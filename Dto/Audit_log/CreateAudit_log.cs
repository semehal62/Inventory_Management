using Inventory_management_System.Models;

namespace Inventory_management_System.Dto.Audit_log
{
    public class CreateAudit_log
    {

        public required AI_Status AI_Status { get; set; }
        public required String Anomalies_Detedced { get; set; }
        public required int SoldId { get; set; }
        public required string Explanation { get; set; }

    }
}

using Inventory_management_System.Models;

namespace Inventory_management_System.Dto.SaleAuditResultDto
{
    public class SaleAuditResultDto
    {
        public AI_Status Status { get; set; }
        public string Anomalies_Detected { get; set; } = "";
        public string Explanation { get; set; } = "";
    }
}

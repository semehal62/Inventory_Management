using Inventory_management_System.Models;

namespace Inventory_management_System.Dto.Sales
{
    public class CreateSale
    {
        public int BaseUserId { get; set; }
        public int ItemsId { get; set; }
        public required int Quantity_Sold { get; set; }
        public float Total_prices { get; set; }

    }
}

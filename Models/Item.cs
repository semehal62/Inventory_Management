namespace Inventory_management_System.Models
{
    public class Item: BaseEntity
    {
        public required int Quantity { get; set; }
        public required int Price { get; set; }
        public Manager ? Manager { get; set; }
        public int? MangerId { get; set; }
        public DateTime Enter_date { get; set; }
        
    }
}

namespace Inventory_management_System.Models
{
    public class Item 
    {
        public  int Id { get; set; }
        public required string Name { get; set; }
        public required int Quantity { get; set; }
        public required int Price { get; set; }
        public Manager ? Manager { get; set; }
        public int? MangerId { get; set; }
        public DateTime Enter_date { get; set; }
        
    }
}

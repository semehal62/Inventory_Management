namespace Inventory_management_System.Dto.Item
{
    public class CreateItem
    {
        public required String Name { get; set; }
        public required int Quantity { get; set; }
        public required int Prices {  get; set;}
        public required int ManagerId { get; set; }
    }
}

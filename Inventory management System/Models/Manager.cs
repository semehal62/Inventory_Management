namespace Inventory_management_System.Models
{
    public class Manager 
    {
        public int Id { get; set; }
        public BaseUser? User { get; set; }
        public required int  BaseUserId { get; set; }
    }
}

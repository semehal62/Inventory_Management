namespace Inventory_management_System.Models
{
    public class Manager
    {
        public  int id {  get; set; }
        public BaseUser ? BaseUser { get; set; }
        public required int BaseUserId { get; set; }
    }
}

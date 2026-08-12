namespace Inventory_management_System.Models
{
    public class Employee
    {
        public  int Id {  get; set; }
        public BaseUser? BaseUser { get; set; }
        public required  int BaseUserId {  get; set; }
    }
}
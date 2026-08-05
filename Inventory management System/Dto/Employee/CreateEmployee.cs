namespace Inventory_management_System.Dto.Employee
{
    public class CreateEmployee
    {
        public required String Name { get; set; }
        public required String Username { get; set; }
        public required String Password { get; set; }
        public required String Role { get; set; }
    }
}

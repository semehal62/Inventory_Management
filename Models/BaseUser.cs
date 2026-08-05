using Microsoft.AspNetCore.Identity;

namespace Inventory_management_System.Models
{
    public class BaseUser
    {
        public int  Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public  required string Role {  get; set; }

    }
}

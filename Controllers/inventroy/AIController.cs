using Inventory_management_System.Models;
using Inventory_management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_management_System.Controllers.inventroy

{
    [ApiController]
    [Route("api/[Controller]")]
    public class AIController:ControllerBase    
    {
        private readonly IAIServices _aiservices;

        public AIController(IAIServices aiservices)
        {
            _aiservices = aiservices;
        }
        //[HttpGet("test")]
        //public async Task<IActionResult> Test()
        //{
        //    var sale = new Sale
        //    {
        //        Id = 1,
        //        BaseUserId = 10,
        //        ItemsId = 5,
        //        Quantity_Sold = 50,
        //        Total_prices = 50000,
        //        Sold_date = DateTime.Now,
        //        TrackStatus = TrackStatus.Pending
        //    };
        //    var result = await _aiservices.AnalyzeSaleAsync(sale);
        //    return Ok(result);
        //}
    
    
    }

}

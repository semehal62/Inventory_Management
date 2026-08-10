namespace Inventory_management_System.Services;
using Inventory_management_System.Models;
using Inventory_management_System.Dto.SaleAuditResultDto;



    public interface IAIServices
    {
       
       
        Task<SaleAuditResultDto> AnalyzeSaleAsync(Sale sale);
        //Task<string> AnalyzeSaleAsync(string data);
        //Task<string> GenerateInventoryRecommendationAsync(string data);

    }


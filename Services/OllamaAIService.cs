
using Inventory_management_System.Dto.SaleAuditResultDto;
using Inventory_management_System.Models;
using System.Text;
using System.Text.Json;

namespace Inventory_management_System.Services
{
    public class OllamaAIService : IAIServices
    {
        private readonly HttpClient _httpClient;

        public OllamaAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SaleAuditResultDto> AnalyzeSaleAsync(Sale sale)
        {
            var prompt = $$$"""
                You are an AI audit assistant for an inventory management system.

                Analyze the following sale:

                Sale ID: {sale.Id}
                Employee ID: {sale.EmployeeId}
                Item ID: {sale.ItemsId}
                Quantity Sold: {sale.Quantity_Sold}
                Total Price: {sale.Total_prices}
                Sale Date: {sale.Sold_date}
                Tracking Status: {sale.TrackStatus}

                Classify this sale as exactly one of:

                Approved
                Warning
                Flagged

                Approved means the sale appears normal.
                Warning means there may be something unusual.
                Flagged means the sale appears highly suspicious.

                Return your response in this exact JSON format:

                {{
                    "status": "Approved",
                    "anomaliesDetected": "None",
                    "explanation": "The sale appears normal."
                }}

                Do not include markdown.
                Do not include any text outside the JSON.
                """;

            var request = new
            {
                model = "deepseek-r1:latest",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(request);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "/api/generate",
                content
            );

            response.EnsureSuccessStatusCode();

            var ollamaResponse = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(ollamaResponse);

            var aiText = document.RootElement
                .GetProperty("response")
                .GetString();

            if (string.IsNullOrWhiteSpace(aiText))
            {
                throw new Exception("DeepSeek returned an empty response.");
            }

            return ParseAIResponse(aiText);
        }

        private SaleAuditResultDto ParseAIResponse(string aiText)
        {
            Console.WriteLine("AI RESPONSE:");
            Console.WriteLine(aiText);

            using var document = JsonDocument.Parse(aiText);

            var root = document.RootElement;

            var statusText = root
                .GetProperty("status")
                .GetString();

            var anomalies = root
                .GetProperty("anomaliesDetected")
                .GetString();

            var explanation = root
                .GetProperty("explanation")
                .GetString();

            if (!Enum.TryParse<AI_Status>(
                statusText,
                true,
                out var status))
            {
                throw new Exception(
                    $"Invalid AI Status: {statusText}"
                );
            }

            return new SaleAuditResultDto
            {
                Status = status,
                Anomalies_Detected = anomalies ?? "None",
                Explanation = explanation ?? ""
            };
        }
    }
}

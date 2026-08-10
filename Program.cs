using Inventory_management_System.Models;
using Inventory_management_System.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    });



// Configure memory cache with options
builder.Services.AddMemoryCache();
builder.Services.Configure<MemoryCacheOptions>(options =>
{
    options.SizeLimit = 1000; // Maximum number of entries
    options.CompactionPercentage = 0.25; // Remove 25% when limit hit
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<InventoryDBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddHttpClient<IAIServices, OllamaAIService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.Timeout = TimeSpan.FromMinutes(5);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

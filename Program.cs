using Inventory_management_System.Models;
using Inventory_management_System.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<InventoryDBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddHttpClient<IAIServices, OllamaAIService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddScoped<IPasswordHasher<BaseUser>, PasswordHasher<BaseUser>>();
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

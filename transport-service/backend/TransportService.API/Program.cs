using Microsoft.EntityFrameworkCore;
using TransportService.API.Data;
using TransportService.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database context
builder.Services.AddDbContext<TransportDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HTTP Client for external services
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    var accountServiceUrl = builder.Configuration["ACCOUNT_SERVICE_URL"] ?? "http://account-service-backend:5001";
    client.BaseAddress = new Uri(accountServiceUrl);
});

builder.Services.AddHttpClient<IOfferServiceClient, OfferServiceClient>(client =>
{
    var offerServiceUrl = builder.Configuration["OFFER_SERVICE_URL"] ?? "http://offer-service-backend:5002";
    client.BaseAddress = new Uri(offerServiceUrl);
});

builder.Services.AddHttpClient<IPurchaseServiceClient, PurchaseServiceClient>(client =>
{
    var purchaseServiceUrl = builder.Configuration["PURCHASE_SERVICE_URL"] ?? "http://purchase-service-backend:5004";
    client.BaseAddress = new Uri(purchaseServiceUrl);
});

// Services
builder.Services.AddScoped<ITransportService, TransportService.API.Services.TransportService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TransportDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
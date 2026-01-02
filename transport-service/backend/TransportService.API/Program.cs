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
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IOfferServiceClient, OfferServiceClient>();
builder.Services.AddHttpClient<IPurchaseServiceClient, PurchaseServiceClient>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransportService, TransportService.API.Services.TransportService>();
builder.Services.AddScoped<IOfferServiceClient, OfferServiceClient>();
builder.Services.AddScoped<IPurchaseServiceClient, PurchaseServiceClient>();
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
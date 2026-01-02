using SearchService.API.Models;
using SearchService.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure Elasticsearch
var elasticsearchUrl = builder.Configuration.GetConnectionString("Elasticsearch") ?? "http://localhost:9200";
builder.Services.AddSingleton<IElasticsearchService>(provider => 
    new ElasticsearchService(elasticsearchUrl));

// Configure RabbitMQ
var rabbitMQConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://admin:admin123@localhost:5672/";
builder.Services.AddSingleton<IRabbitMQConsumerService>(provider => 
    new RabbitMQConsumerService(
        rabbitMQConnectionString, 
        provider.GetRequiredService<IElasticsearchService>(),
        provider.GetRequiredService<ILogger<RabbitMQConsumerService>>()
    ));

// Add hosted service for RabbitMQ consumer
builder.Services.AddHostedService<RabbitMQHostedService>();

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

// Initialize Elasticsearch index
using (var scope = app.Services.CreateScope())
{
    var elasticsearchService = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
    await elasticsearchService.InitializeIndexAsync();
}

app.Run();
using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// specific repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITickerRepository, TickerRepository>();
builder.Services.AddScoped<IRecommendationBasketRepository, RecommendationBasketRepository>();
builder.Services.AddScoped<ITradingAccountRepository, TradingAccountRepository>();
builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IDistributionRepository, DistributionRepository>();
builder.Services.AddScoped<ITaxEventRepository, TaxEventRepository>();
builder.Services.AddScoped<IRebalancingRepository, RebalancingRepository>();



builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();


// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ItauTopFiveDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//builder.Services.AddSingleton<Application.Services.IKafkaProducer, Application.Services.KafkaProducer>();
// builder.Services.AddScoped<Application.Services.IIrEventService, Application.Services.IrEventService>();





// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

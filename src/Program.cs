using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:7239");
//REPOSITÓRIOS
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITickerRepository, TickerRepository>();
builder.Services.AddScoped<IRecommendationBasketRepository, RecommendationBasketRepository>();
builder.Services.AddScoped<ITradingAccountRepository, TradingAccountRepository>();
builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IDistributionRepository, DistributionRepository>();
builder.Services.AddScoped<ITaxEventRepository, TaxEventRepository>();
builder.Services.AddScoped<IRebalancingRepository, RebalancingRepository>();
builder.Services.AddScoped<IContributionHistoryRepository, ContributionHistoryRepository>();

//SERVICES
builder.Services.AddScoped<IParserB3CotHist, ParserB3CotHist>();

builder.Services.AddScoped<IRebalancingEngineService, RebalancingEngineService>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IRecommendationBasketService, RecommendationBasketService>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IPurchaseEngineService, PurchaseEngineService>();
builder.Services.AddScoped<IMasterAccountService, MasterAccountService>();


builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();



builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ItauTopFiveDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .SetIsOriginAllowed(_ => true) 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); 
});


var app = builder.Build();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
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

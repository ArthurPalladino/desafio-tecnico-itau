using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ItauTopFiveDbContext : DbContext
    {
        public ItauTopFiveDbContext(DbContextOptions<ItauTopFiveDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<TradingAccount> TradingAccounts { get; set; } = null!;
        public DbSet<Ticker> Tickers { get; set; } = null!; 
        public DbSet<RecommendationBasket> RecommendationBaskets { get; set; } = null!;
        public DbSet<BasketItem> BasketItems { get; set; } = null!;
        public DbSet<Custody> Custodies { get; set; } = null!;
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        public DbSet<Distribution> Distributions { get; set; } = null!;
        public DbSet<TaxEvent> TaxEvents { get; set; } = null!;
        public DbSet<Rebalancing> Rebalancings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItauTopFiveDbContext).Assembly);
        }
    }
}
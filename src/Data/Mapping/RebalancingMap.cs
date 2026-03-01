using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class RebalancingMap : IEntityTypeConfiguration<Rebalancing>
    {
        public void Configure(EntityTypeBuilder<Rebalancing> builder)
        {
            builder.ToTable("tb_rebalancings");
            
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("id_rebalancing");

            builder.Property(r => r.CustomerId)
                .HasColumnName("id_customer");

            builder.Property(r => r.SellTicker)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("st_sell_ticker");

            builder.Property(r => r.BuyTicker)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("st_buy_ticker");

            builder.Property(r => r.Quantity)
                .HasColumnType("DECIMAL(18,4)") 
                .HasColumnName("vl_quantity");

            builder.Property(r => r.RebalancingDate)
                .HasColumnName("dt_rebalancing");

            builder.Property(r => r.Type)
                .HasColumnName("tp_rebalancing_type");
        }
    }
}
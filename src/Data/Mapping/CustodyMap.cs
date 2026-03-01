using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class CustodyMap : IEntityTypeConfiguration<Custody>
    {
        public void Configure(EntityTypeBuilder<Custody> builder)
        {
            builder.ToTable("tb_custodies");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id_custody");

            builder.Property(c => c.CustomerId)
                .HasColumnName("id_customer");

            builder.Property(c => c.TradingAccountId)
                .HasColumnName("id_trading_account");

            builder.Property(c => c.Symbol)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("st_symbol");

            builder.Property(c => c.Quantity)
                .HasColumnName("vl_quantity");

            builder.Property(c => c.AveragePrice)
                .HasColumnType("DECIMAL(10,2)")
                .HasColumnName("vl_average_price");
        }
    }
}
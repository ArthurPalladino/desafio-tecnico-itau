using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class TickerMap : IEntityTypeConfiguration<Ticker>
    {
        public void Configure(EntityTypeBuilder<Ticker> builder)
        {
            builder.ToTable("tb_tickers"); 

            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Id)
                .HasColumnName("id_ticker");

            builder.Property(t => t.Symbol)
                .IsRequired()
                .HasMaxLength(12)
                .HasColumnName("st_symbol");

            builder.Property(t => t.CurrentPrice)
                .HasColumnType("DECIMAL(10,2)")
                .HasColumnName("vl_current_price");

            builder.Property(t => t.PriceDate)
                .HasColumnName("dt_price_date");

        }
    }
}
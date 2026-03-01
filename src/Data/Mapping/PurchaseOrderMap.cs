using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class PurchaseOrderMap : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("tb_purchase_orders");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id_purchase_order");

            builder.Property(o => o.MasterAccountId)
                .HasColumnName("id_master_account");

            builder.Property(o => o.Symbol)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("st_symbol");

            builder.Property(o => o.Quantity)
                .HasColumnName("vl_quantity");

            builder.Property(o => o.UnitPrice)
                .HasColumnType("DECIMAL(10,2)")
                .HasColumnName("vl_unit_price");

            builder.Property(o => o.ExecutionDate)
                .HasColumnName("dt_execution_date");

            builder.Property(o => o.MarketType)
                .HasColumnName("tp_market_type");
        }
    }
}
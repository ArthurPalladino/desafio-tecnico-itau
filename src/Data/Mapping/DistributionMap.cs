using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class DistributionMap : IEntityTypeConfiguration<Distribution>
    {
        public void Configure(EntityTypeBuilder<Distribution> builder)
        {
            builder.ToTable("tb_distributions");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("id_distribution");

            builder.Property(d => d.PurchaseOrderId)
                .HasColumnName("id_purchase_order");

            builder.Property(d => d.ChildAccountId)
                .HasColumnName("id_child_account");

            builder.Property(d => d.Quantity)
                .HasColumnName("vl_quantity");

            builder.Property(d => d.ExecutionPrice)
                .HasColumnType("DECIMAL(10,2)")
                .HasColumnName("vl_execution_price");
        }
    }
}
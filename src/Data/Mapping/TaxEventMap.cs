using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class TaxEventMap : IEntityTypeConfiguration<TaxEvent>
    {
        public void Configure(EntityTypeBuilder<TaxEvent> builder)
        {
            builder.ToTable("tb_tax_events");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id_tax_event");

            builder.Property(e => e.AlreadyInKafka)
                .HasColumnName("st_already_in_kafka")
                .HasDefaultValue(false);

            builder.Property(e => e.CustomerId)
                .HasColumnName("id_customer");

            builder.Property(e => e.BaseAmount)
                .HasColumnType("DECIMAL(10,2)")
                .HasColumnName("vl_base_amount");

            builder.Property(e => e.TaxAmount)
                .HasColumnType("DECIMAL(10,2)")
                .HasColumnName("vl_tax_amount");

            builder.Property(e => e.EventDate)
                .HasColumnName("dt_event_date");

            builder.Property(e => e.Type)
                .HasColumnName("tp_tax_type");
        }
    }
}
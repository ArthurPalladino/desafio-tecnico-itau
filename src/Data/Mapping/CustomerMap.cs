using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("tb_customers");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("id_customer");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("st_name");

            builder.Property(c => c.Cpf)
                .IsRequired()
                .HasMaxLength(11)
                .HasColumnName("st_cpf");

            builder.Property(c => c.Email)
                .HasMaxLength(100)
                .HasColumnName("st_email");

            builder.Property(c => c.IsActive) 
                .HasColumnName("fl_active");

            builder.Property(c => c.MonthlyContribution)
                .HasColumnType("DECIMAL(10,2)")
                .HasColumnName("vl_monthly_contribution");

            builder.Property(c => c.SubscriptionDate)
                .HasColumnName("dt_subscription_date");

            builder.HasIndex(c => c.Cpf)
                .IsUnique()
                .HasDatabaseName("ix_customer_cpf");

            builder.HasData(new 
                {
                    Id = 1, // ID Fixo para a Master
                    Name = "Itaú Corretora Master",
                    Cpf = "00000000000", 
                    Email = "master@itau.com.br",
                    MonthlyContribution = 0m,
                    SubscriptionDate = new DateTime(2024, 1, 1),
                    IsActive = true
                });
        }
    }
}
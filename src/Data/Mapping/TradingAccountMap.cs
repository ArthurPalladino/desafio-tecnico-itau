using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class TradingAccountMap : IEntityTypeConfiguration<TradingAccount>
    {
        public void Configure(EntityTypeBuilder<TradingAccount> builder)
        {
            builder.ToTable("tb_trading_accounts");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id_trading_account");

            builder.Property(c => c.CustomerId)
                .HasColumnName("id_customer");

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("st_description");

            builder.Property(c => c.Type)
                .HasColumnName("tp_account_type");

            builder.HasMany(c => c.Custodies)
                .WithOne()
                .HasForeignKey(cd => cd.TradingAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
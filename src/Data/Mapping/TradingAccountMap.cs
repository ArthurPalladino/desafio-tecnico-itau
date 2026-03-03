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

            builder.HasOne(t => t.Customer)
                .WithOne(c => c.TradingAccount) 
                .HasForeignKey<TradingAccount>(t => t.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.AccountNumber)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("st_account_number");

            builder.Property(c => c.Type)
                .HasColumnName("tp_account_type");
            
            builder.Property(x => x.Balance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);
            builder.HasMany(c => c.Custodies)
                .WithOne()
                .HasForeignKey(cd => cd.TradingAccountId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasData(new 
            {
                Id = 1,
                CustomerId = 1, 
                AccountNumber = "000000-0", 
                Balance = 0m,
                Type = AccountType.Master 
            });
        }
    }
}
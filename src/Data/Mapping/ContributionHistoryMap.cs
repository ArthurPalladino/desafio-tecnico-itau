using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContributionHistoryMap : IEntityTypeConfiguration<ContributionHistory>
{
    public void Configure(EntityTypeBuilder<ContributionHistory> builder)
    {
        builder.ToTable("tb_contribution_history");

        // Chave Primária
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OldValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.NewValue)
            .HasPrecision(18, 2)
            .IsRequired();

        // Data da alteração para controle da RN-012 e RN-013
        builder.Property(x => x.AlterationDate)
            .IsRequired();

        builder.HasOne(x => x.Customer)
            .WithMany(c => c.ContributionHistories)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
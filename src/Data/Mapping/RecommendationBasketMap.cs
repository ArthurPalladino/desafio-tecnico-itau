using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class RecommendationBasketMap : IEntityTypeConfiguration<RecommendationBasket>
    {
        public void Configure(EntityTypeBuilder<RecommendationBasket> builder)
        {
            builder.ToTable("tb_recommendation_baskets");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasColumnName("id_basket");

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("st_name");

            builder.Property(b => b.IsActive)
                .IsRequired()
                .HasColumnName("fl_active");

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasColumnName("dt_created_at");

            builder.Property(b => b.DeactivatedAt)
                .HasColumnName("dt_deactivated_at");


            builder.HasMany(b => b.Itens)
            .WithOne()
            .HasForeignKey(ci => ci.RecommendationBasketId)
            .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(RecommendationBasket.Itens));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
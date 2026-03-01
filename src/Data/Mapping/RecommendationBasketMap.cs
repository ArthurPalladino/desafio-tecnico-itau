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


            builder.HasMany(b => b.Items)
                .WithOne()
                .HasForeignKey("id_basket") 
                .OnDelete(DeleteBehavior.Cascade);

            // Acesso ao campo privado (opcional, mas bom para DDD)
            var navigation = builder.Metadata.FindNavigation(nameof(RecommendationBasket.Items));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mapping
{
    public class BasketItemMap : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.ToTable("tb_basket_itens");

            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Id)
                .HasColumnName("id_basket_item");

            builder.Property(ci => ci.RecommendationBasketId)
                .HasColumnName("id_basket");

            builder.Property(ci => ci.Symbol)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("st_symbol");

            builder.Property(ci => ci.Percentage)
                .HasColumnType("DECIMAL(5,2)")
                .HasColumnName("vl_percentage");
        }
    }
}
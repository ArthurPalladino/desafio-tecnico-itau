using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItauTopFive.Migrations
{
    /// <inheritdoc />
    public partial class FixBasketItemForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_basket_items_tb_recommendation_baskets_id_basket1",
                table: "tb_basket_items");

            migrationBuilder.DropIndex(
                name: "IX_tb_basket_items_id_basket1",
                table: "tb_basket_items");

            migrationBuilder.DropColumn(
                name: "id_basket1",
                table: "tb_basket_items");

            migrationBuilder.CreateIndex(
                name: "IX_tb_basket_items_id_basket",
                table: "tb_basket_items",
                column: "id_basket");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_basket_items_tb_recommendation_baskets_id_basket",
                table: "tb_basket_items",
                column: "id_basket",
                principalTable: "tb_recommendation_baskets",
                principalColumn: "id_basket",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_basket_items_tb_recommendation_baskets_id_basket",
                table: "tb_basket_items");

            migrationBuilder.DropIndex(
                name: "IX_tb_basket_items_id_basket",
                table: "tb_basket_items");

            migrationBuilder.AddColumn<int>(
                name: "id_basket1",
                table: "tb_basket_items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tb_basket_items_id_basket1",
                table: "tb_basket_items",
                column: "id_basket1");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_basket_items_tb_recommendation_baskets_id_basket1",
                table: "tb_basket_items",
                column: "id_basket1",
                principalTable: "tb_recommendation_baskets",
                principalColumn: "id_basket",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

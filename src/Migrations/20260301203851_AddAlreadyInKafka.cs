using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItauTopFive.Migrations
{
    /// <inheritdoc />
    public partial class AddAlreadyInKafka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "st_already_in_kafka",
                table: "tb_tax_events",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "st_already_in_kafka",
                table: "tb_tax_events");
        }
    }
}

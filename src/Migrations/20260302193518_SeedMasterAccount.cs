using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItauTopFive.Migrations
{
    /// <inheritdoc />
    public partial class SeedMasterAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tb_customers",
                columns: new[] { "id_customer", "st_cpf", "st_email", "fl_active", "vl_monthly_contribution", "st_name", "dt_subscription_date" },
                values: new object[] { 1, "00000000000", "master@itau.com.br", true, 0m, "Itaú Corretora Master", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "tb_trading_accounts",
                columns: new[] { "id_trading_account", "st_account_number", "id_customer", "tp_account_type" },
                values: new object[] { 1, "000000-0", 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tb_trading_accounts",
                keyColumn: "id_trading_account",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "tb_customers",
                keyColumn: "id_customer",
                keyValue: 1);
        }
    }
}

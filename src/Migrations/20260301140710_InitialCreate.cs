using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItauTopFive.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_customers",
                columns: table => new
                {
                    id_customer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    st_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    st_cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    st_email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fl_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    vl_monthly_contribution = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    dt_subscription_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_customers", x => x.id_customer);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_distributions",
                columns: table => new
                {
                    id_distribution = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_purchase_order = table.Column<int>(type: "int", nullable: false),
                    id_child_account = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vl_quantity = table.Column<int>(type: "int", nullable: false),
                    vl_execution_price = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    DistributedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_distributions", x => x.id_distribution);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_purchase_orders",
                columns: table => new
                {
                    id_purchase_order = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_master_account = table.Column<int>(type: "int", nullable: false),
                    st_symbol = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vl_quantity = table.Column<int>(type: "int", nullable: false),
                    vl_unit_price = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    dt_execution_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    tp_market_type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_purchase_orders", x => x.id_purchase_order);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_rebalancings",
                columns: table => new
                {
                    id_rebalancing = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_customer = table.Column<int>(type: "int", nullable: false),
                    st_sell_ticker = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    st_buy_ticker = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vl_quantity = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    dt_rebalancing = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    tp_rebalancing_type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_rebalancings", x => x.id_rebalancing);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_recommendation_baskets",
                columns: table => new
                {
                    id_basket = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    st_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fl_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    dt_created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    dt_deactivated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_recommendation_baskets", x => x.id_basket);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_tickers",
                columns: table => new
                {
                    id_ticker = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    st_symbol = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vl_current_price = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    dt_created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    dt_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_tickers", x => x.id_ticker);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_tax_events",
                columns: table => new
                {
                    id_tax_event = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_customer = table.Column<int>(type: "int", nullable: false),
                    vl_base_amount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    vl_tax_amount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    dt_event_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    tp_tax_type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_tax_events", x => x.id_tax_event);
                    table.ForeignKey(
                        name: "FK_tb_tax_events_tb_customers_id_customer",
                        column: x => x.id_customer,
                        principalTable: "tb_customers",
                        principalColumn: "id_customer",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_trading_accounts",
                columns: table => new
                {
                    id_trading_account = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_customer = table.Column<int>(type: "int", nullable: false),
                    st_description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tp_account_type = table.Column<int>(type: "int", nullable: false),
                    CustomerId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_trading_accounts", x => x.id_trading_account);
                    table.ForeignKey(
                        name: "FK_tb_trading_accounts_tb_customers_CustomerId1",
                        column: x => x.CustomerId1,
                        principalTable: "tb_customers",
                        principalColumn: "id_customer",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_trading_accounts_tb_customers_id_customer",
                        column: x => x.id_customer,
                        principalTable: "tb_customers",
                        principalColumn: "id_customer",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_basket_items",
                columns: table => new
                {
                    id_basket_item = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_basket = table.Column<int>(type: "int", nullable: false),
                    st_symbol = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vl_percentage = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    id_basket1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_basket_items", x => x.id_basket_item);
                    table.ForeignKey(
                        name: "FK_tb_basket_items_tb_recommendation_baskets_id_basket1",
                        column: x => x.id_basket1,
                        principalTable: "tb_recommendation_baskets",
                        principalColumn: "id_basket",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_custodies",
                columns: table => new
                {
                    id_custody = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_customer = table.Column<int>(type: "int", nullable: false),
                    id_trading_account = table.Column<int>(type: "int", nullable: false),
                    st_symbol = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vl_quantity = table.Column<int>(type: "int", nullable: false),
                    vl_average_price = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_custodies", x => x.id_custody);
                    table.ForeignKey(
                        name: "FK_tb_custodies_tb_customers_id_customer",
                        column: x => x.id_customer,
                        principalTable: "tb_customers",
                        principalColumn: "id_customer",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_custodies_tb_trading_accounts_id_trading_account",
                        column: x => x.id_trading_account,
                        principalTable: "tb_trading_accounts",
                        principalColumn: "id_trading_account",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tb_basket_items_id_basket1",
                table: "tb_basket_items",
                column: "id_basket1");

            migrationBuilder.CreateIndex(
                name: "IX_tb_custodies_id_customer",
                table: "tb_custodies",
                column: "id_customer");

            migrationBuilder.CreateIndex(
                name: "IX_tb_custodies_id_trading_account",
                table: "tb_custodies",
                column: "id_trading_account");

            migrationBuilder.CreateIndex(
                name: "ix_customer_cpf",
                table: "tb_customers",
                column: "st_cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_tax_events_id_customer",
                table: "tb_tax_events",
                column: "id_customer");

            migrationBuilder.CreateIndex(
                name: "IX_tb_trading_accounts_CustomerId1",
                table: "tb_trading_accounts",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_tb_trading_accounts_id_customer",
                table: "tb_trading_accounts",
                column: "id_customer",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_basket_items");

            migrationBuilder.DropTable(
                name: "tb_custodies");

            migrationBuilder.DropTable(
                name: "tb_distributions");

            migrationBuilder.DropTable(
                name: "tb_purchase_orders");

            migrationBuilder.DropTable(
                name: "tb_rebalancings");

            migrationBuilder.DropTable(
                name: "tb_tax_events");

            migrationBuilder.DropTable(
                name: "tb_tickers");

            migrationBuilder.DropTable(
                name: "tb_recommendation_baskets");

            migrationBuilder.DropTable(
                name: "tb_trading_accounts");

            migrationBuilder.DropTable(
                name: "tb_customers");
        }
    }
}

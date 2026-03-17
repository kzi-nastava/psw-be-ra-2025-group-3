using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payments");

            migrationBuilder.CreateTable(
                name: "BundlePurchaseRecords",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    BundleId = table.Column<long>(type: "bigint", nullable: false),
                    PriceAc = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundlePurchaseRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Items = table.Column<string>(type: "jsonb", nullable: false),
                    BundleItems = table.Column<string>(type: "jsonb", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourPurchaseRecords",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    PriceAc = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPurchaseRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourPurchaseTokens",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPurchaseTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BundlePurchaseRecords_TouristId",
                schema: "payments",
                table: "BundlePurchaseRecords",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPurchaseRecords_TouristId",
                schema: "payments",
                table: "TourPurchaseRecords",
                column: "TouristId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BundlePurchaseRecords",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "ShoppingCarts",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "TourPurchaseRecords",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "TourPurchaseTokens",
                schema: "payments");
        }
    }
}

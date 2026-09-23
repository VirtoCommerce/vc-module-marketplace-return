using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.MarketplaceReturn.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class SellerReturnInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SellerReturn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ReturnId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SellerId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SellerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerReturn", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellerReturn_ReturnId",
                table: "SellerReturn",
                column: "ReturnId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellerReturn");
        }
    }
}

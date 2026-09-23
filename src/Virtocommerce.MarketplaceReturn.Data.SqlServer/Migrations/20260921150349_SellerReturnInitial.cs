using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtocommerce.MarketplaceReturn.Data.SqlServer.Migrations
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
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ReturnId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SellerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SellerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
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

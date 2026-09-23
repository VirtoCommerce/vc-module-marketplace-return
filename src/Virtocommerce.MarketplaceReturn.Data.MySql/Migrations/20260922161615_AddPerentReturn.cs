using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtocommerce.MarketplaceReturn.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class AddPerentReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentReturnId",
                table: "SellerReturn",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentReturnId",
                table: "SellerReturn");
        }
    }
}

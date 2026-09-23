using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtocommerce.MarketplaceReturn.Data.SqlServer.Migrations
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
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
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

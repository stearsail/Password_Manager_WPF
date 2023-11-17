using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Password_manager.Migrations
{
    /// <inheritdoc />
    public partial class AccountSaltAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountSalt",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountSalt",
                table: "Accounts");
        }
    }
}

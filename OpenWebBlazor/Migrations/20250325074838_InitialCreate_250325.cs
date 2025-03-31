using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWebBlazor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_250325 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "WebUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "WebUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebUsers_UserName",
                table: "WebUsers",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WebUsers_UserName",
                table: "WebUsers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "WebUsers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "WebUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

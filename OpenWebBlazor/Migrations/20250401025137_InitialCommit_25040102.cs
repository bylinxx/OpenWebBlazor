using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWebBlazor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit_25040102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShow",
                table: "WebMenus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShow",
                table: "WebMenus");
        }
    }
}

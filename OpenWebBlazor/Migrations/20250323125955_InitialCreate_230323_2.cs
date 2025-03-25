using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWebBlazor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_230323_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "WebUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "WebUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scholar.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardLayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DashboardLayout",
                table: "Institutes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DashboardLayout",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DashboardLayout",
                table: "Institutes");

            migrationBuilder.DropColumn(
                name: "DashboardLayout",
                table: "AspNetUsers");
        }
    }
}

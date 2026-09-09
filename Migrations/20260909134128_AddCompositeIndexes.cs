using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scholar.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Test_InstituteId",
                table: "Test");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TopicId",
                table: "Questions");

            migrationBuilder.CreateIndex(
                name: "IX_Test_InstituteId_IsActive",
                table: "Test",
                columns: new[] { "InstituteId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TopicId_Type",
                table: "Questions",
                columns: new[] { "TopicId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Test_InstituteId_IsActive",
                table: "Test");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TopicId_Type",
                table: "Questions");

            migrationBuilder.CreateIndex(
                name: "IX_Test_InstituteId",
                table: "Test",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TopicId",
                table: "Questions",
                column: "TopicId");
        }
    }
}

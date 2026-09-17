using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scholar.Migrations
{
    /// <inheritdoc />
    public partial class MakeClassSectionsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the join table first so existing Class.SectionId links can be
            // migrated into it before the column is dropped.
            migrationBuilder.CreateTable(
                name: "ClassSection",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSection", x => new { x.ClassId, x.SectionId });
                    table.ForeignKey(
                        name: "FK_ClassSection_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSection_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSection_SectionId",
                table: "ClassSection",
                column: "SectionId");

            // Preserve each class's existing single section as its first many-to-many link.
            migrationBuilder.Sql(
                "INSERT INTO [ClassSection] ([ClassId], [SectionId]) SELECT [Id], [SectionId] FROM [Class];");

            migrationBuilder.DropForeignKey(
                name: "FK_Class_Sections_SectionId",
                table: "Class");

            migrationBuilder.DropIndex(
                name: "IX_Class_SectionId",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Class");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassSection");

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Class_SectionId",
                table: "Class",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Sections_SectionId",
                table: "Class",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

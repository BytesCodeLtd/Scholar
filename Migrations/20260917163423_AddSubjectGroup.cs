using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scholar.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstituteId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectGroups_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectGroups_Institutes_InstituteId",
                        column: x => x.InstituteId,
                        principalTable: "Institutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectGroupSection",
                columns: table => new
                {
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SubjectGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroupSection", x => new { x.SectionId, x.SubjectGroupId });
                    table.ForeignKey(
                        name: "FK_SubjectGroupSection_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectGroupSection_SubjectGroups_SubjectGroupId",
                        column: x => x.SubjectGroupId,
                        principalTable: "SubjectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectGroupSubject",
                columns: table => new
                {
                    InstituteSubjectId = table.Column<int>(type: "int", nullable: false),
                    SubjectGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroupSubject", x => new { x.InstituteSubjectId, x.SubjectGroupId });
                    table.ForeignKey(
                        name: "FK_SubjectGroupSubject_InstituteSubjects_InstituteSubjectId",
                        column: x => x.InstituteSubjectId,
                        principalTable: "InstituteSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectGroupSubject_SubjectGroups_SubjectGroupId",
                        column: x => x.SubjectGroupId,
                        principalTable: "SubjectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_ClassId",
                table: "SubjectGroups",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_InstituteId",
                table: "SubjectGroups",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroupSection_SubjectGroupId",
                table: "SubjectGroupSection",
                column: "SubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroupSubject_SubjectGroupId",
                table: "SubjectGroupSubject",
                column: "SubjectGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectGroupSection");

            migrationBuilder.DropTable(
                name: "SubjectGroupSubject");

            migrationBuilder.DropTable(
                name: "SubjectGroups");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scholar.Migrations
{
    /// <inheritdoc />
    public partial class ExpandStudentAdmissionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionDate",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AdmissionDiscount",
                table: "Students",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionHead",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionSession",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AsOnDate",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BFormCnic",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardId",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Caste",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcessionType",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConcessionValidTill",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FamilyNo",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherCnic",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherOccupation",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherPhone",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherPhotoUrl",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianAddress",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianEmail",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianOccupation",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianPhone",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianPhotoUrl",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianRelation",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianType",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFamilyHead",
                table: "Students",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastClassAttended",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherCnic",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherOccupation",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherPhone",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherPhotoUrl",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousSchoolDetails",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefBy",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TuitionFeeHead",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Weight",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsappNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AdmissionDiscount",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AdmissionHead",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AdmissionNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AdmissionSession",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AsOnDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "BFormCnic",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Caste",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ConcessionType",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ConcessionValidTill",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FamilyNo",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FatherCnic",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FatherOccupation",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FatherPhone",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FatherPhotoUrl",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianAddress",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianEmail",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianOccupation",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianPhone",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianPhotoUrl",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianRelation",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianType",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsFamilyHead",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LastClassAttended",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MotherCnic",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MotherOccupation",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MotherPhone",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MotherPhotoUrl",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PreviousSchoolDetails",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "RefBy",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Religion",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TuitionFeeHead",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "WhatsappNumber",
                table: "Students");
        }
    }
}

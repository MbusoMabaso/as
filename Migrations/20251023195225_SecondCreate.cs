using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CMCS.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Lecturer_LecturerID",
                table: "Claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecturer",
                table: "Lecturer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClaimApproval",
                table: "ClaimApproval");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Lecturer",
                newName: "Lecturers");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameTable(
                name: "ClaimApproval",
                newName: "ClaimApprovals");

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Claims",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Claims",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ClaimApprovals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "ClaimApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalLevel",
                table: "ClaimApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecturers",
                table: "Lecturers",
                column: "LecturerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "DocumentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClaimApprovals",
                table: "ClaimApprovals",
                column: "ApprovalID");

            migrationBuilder.InsertData(
                table: "Lecturers",
                columns: new[] { "LecturerID", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "john.smith@university.ac.za", "John Smith" },
                    { 2, "sarah.johnson@university.ac.za", "Sarah Johnson" },
                    { 3, "michael.brown@university.ac.za", "Michael Brown" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "admin123", "AcademicManager", "admin" },
                    { 2, "coord123", "ProgrammeCoordinator", "coordinator" },
                    { 3, "hr123", "HR", "hr" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ClaimID",
                table: "Documents",
                column: "ClaimID");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovals_ClaimID",
                table: "ClaimApprovals",
                column: "ClaimID");

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimApprovals_Claims_ClaimID",
                table: "ClaimApprovals",
                column: "ClaimID",
                principalTable: "Claims",
                principalColumn: "ClaimID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Lecturers_LecturerID",
                table: "Claims",
                column: "LecturerID",
                principalTable: "Lecturers",
                principalColumn: "LecturerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Claims_ClaimID",
                table: "Documents",
                column: "ClaimID",
                principalTable: "Claims",
                principalColumn: "ClaimID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimApprovals_Claims_ClaimID",
                table: "ClaimApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Lecturers_LecturerID",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Claims_ClaimID",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecturers",
                table: "Lecturers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ClaimID",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClaimApprovals",
                table: "ClaimApprovals");

            migrationBuilder.DropIndex(
                name: "IX_ClaimApprovals_ClaimID",
                table: "ClaimApprovals");

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "LecturerID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "LecturerID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "LecturerID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ApprovalLevel",
                table: "ClaimApprovals");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Lecturers",
                newName: "Lecturer");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameTable(
                name: "ClaimApprovals",
                newName: "ClaimApproval");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ClaimApproval",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "ClaimApproval",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecturer",
                table: "Lecturer",
                column: "LecturerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "DocumentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClaimApproval",
                table: "ClaimApproval",
                column: "ApprovalID");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Lecturer_LecturerID",
                table: "Claims",
                column: "LecturerID",
                principalTable: "Lecturer",
                principalColumn: "LecturerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class RecreateProjectDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Lecturers_LecturerId",
                table: "Claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgrammeCoordinators",
                table: "ProgrammeCoordinators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecturers",
                table: "Lecturers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Claims",
                table: "Claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicManagers",
                table: "AcademicManagers");

            migrationBuilder.RenameTable(
                name: "ProgrammeCoordinators",
                newName: "ProgrammeCoordinator");

            migrationBuilder.RenameTable(
                name: "Lecturers",
                newName: "Lecturer");

            migrationBuilder.RenameTable(
                name: "Claims",
                newName: "Claim");

            migrationBuilder.RenameTable(
                name: "AcademicManagers",
                newName: "AcademicManager");

            migrationBuilder.RenameIndex(
                name: "IX_Claims_LecturerId",
                table: "Claim",
                newName: "IX_Claim_LecturerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgrammeCoordinator",
                table: "ProgrammeCoordinator",
                column: "ProgrammeCoordinatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecturer",
                table: "Lecturer",
                column: "LecturerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Claim",
                table: "Claim",
                column: "ClaimId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicManager",
                table: "AcademicManager",
                column: "AcademicManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claim_Lecturer_LecturerId",
                table: "Claim",
                column: "LecturerId",
                principalTable: "Lecturer",
                principalColumn: "LecturerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claim_Lecturer_LecturerId",
                table: "Claim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgrammeCoordinator",
                table: "ProgrammeCoordinator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecturer",
                table: "Lecturer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Claim",
                table: "Claim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicManager",
                table: "AcademicManager");

            migrationBuilder.RenameTable(
                name: "ProgrammeCoordinator",
                newName: "ProgrammeCoordinators");

            migrationBuilder.RenameTable(
                name: "Lecturer",
                newName: "Lecturers");

            migrationBuilder.RenameTable(
                name: "Claim",
                newName: "Claims");

            migrationBuilder.RenameTable(
                name: "AcademicManager",
                newName: "AcademicManagers");

            migrationBuilder.RenameIndex(
                name: "IX_Claim_LecturerId",
                table: "Claims",
                newName: "IX_Claims_LecturerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgrammeCoordinators",
                table: "ProgrammeCoordinators",
                column: "ProgrammeCoordinatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecturers",
                table: "Lecturers",
                column: "LecturerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Claims",
                table: "Claims",
                column: "ClaimId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicManagers",
                table: "AcademicManagers",
                column: "AcademicManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Lecturers_LecturerId",
                table: "Claims",
                column: "LecturerId",
                principalTable: "Lecturers",
                principalColumn: "LecturerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

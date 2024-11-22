using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClaimModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claim");

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    UserClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimAmount = table.Column<double>(type: "float", nullable: false),
                    HourlyRate = table.Column<double>(type: "float", nullable: false),
                    ClaimStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoursWorked = table.Column<double>(type: "float", nullable: false),
                    FlaggedClaim = table.Column<bool>(type: "bit", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.UserClaimId);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.CreateTable(
                name: "Claim",
                columns: table => new
                {
                    ClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimAmount = table.Column<double>(type: "float", nullable: false),
                    ClaimStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlaggedClaim = table.Column<bool>(type: "bit", nullable: false),
                    HourlyRate = table.Column<double>(type: "float", nullable: false),
                    HoursWorked = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claim", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_Claim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claim_UserId",
                table: "Claim",
                column: "UserId");
        }
    }
}

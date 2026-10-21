using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "survey_assignment",
                columns: table => new
                {
                    survey_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    surveyor_id = table.Column<long>(type: "bigint", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    inspection_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    report_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    survey_status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_assignment", x => x.survey_id);
                    table.ForeignKey(
                        name: "FK_survey_assignment_claim_master_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claim_master",
                        principalColumn: "claim_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_survey_assignment_claim_id",
                table: "survey_assignment",
                column: "claim_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "survey_assignment");
        }
    }
}

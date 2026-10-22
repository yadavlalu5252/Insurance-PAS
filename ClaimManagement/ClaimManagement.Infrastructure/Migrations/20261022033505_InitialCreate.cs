using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "claim_master",
                columns: table => new
                {
                    claim_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claim_number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    claim_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    incident_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    claim_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estimated_loss = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    claim_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_master", x => x.claim_id);
                });

            migrationBuilder.CreateTable(
                name: "claim_assessment",
                columns: table => new
                {
                    assessment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    assessed_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    deductible = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    depreciation = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    non_payable = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    net_payable = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    assessed_by = table.Column<long>(type: "bigint", nullable: false),
                    assessed_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_assessment", x => x.assessment_id);
                    table.ForeignKey(
                        name: "FK_claim_assessment_claim_master_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claim_master",
                        principalColumn: "claim_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "claim_documents",
                columns: table => new
                {
                    document_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    document_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    verification_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uploaded_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_documents", x => x.document_id);
                    table.ForeignKey(
                        name: "FK_claim_documents_claim_master_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claim_master",
                        principalColumn: "claim_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "claim_history",
                columns: table => new
                {
                    history_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    performed_by = table.Column<long>(type: "bigint", nullable: false),
                    action_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_claim_history_claim_master_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claim_master",
                        principalColumn: "claim_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "investigation",
                columns: table => new
                {
                    investigation_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    investigation_number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    investigator_id = table.Column<long>(type: "bigint", nullable: false),
                    investigation_reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    investigation_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    findings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    recommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    report_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    completed_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investigation", x => x.investigation_id);
                    table.ForeignKey(
                        name: "FK_investigation_claim_master_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claim_master",
                        principalColumn: "claim_id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "investigation_evidence",
                columns: table => new
                {
                    evidence_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    investigation_id = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uploaded_by = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investigation_evidence", x => x.evidence_id);
                    table.ForeignKey(
                        name: "FK_investigation_evidence_investigation_investigation_id",
                        column: x => x.investigation_id,
                        principalTable: "investigation",
                        principalColumn: "investigation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_claim_assessment_claim_id",
                table: "claim_assessment",
                column: "claim_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_claim_documents_claim_id",
                table: "claim_documents",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "IX_claim_history_claim_id",
                table: "claim_history",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "IX_claim_master_claim_number",
                table: "claim_master",
                column: "claim_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_investigation_claim_id",
                table: "investigation",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "IX_investigation_investigation_number",
                table: "investigation",
                column: "investigation_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_investigation_evidence_investigation_id",
                table: "investigation_evidence",
                column: "investigation_id");

            migrationBuilder.CreateIndex(
                name: "IX_survey_assignment_claim_id",
                table: "survey_assignment",
                column: "claim_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claim_assessment");

            migrationBuilder.DropTable(
                name: "claim_documents");

            migrationBuilder.DropTable(
                name: "claim_history");

            migrationBuilder.DropTable(
                name: "investigation_evidence");

            migrationBuilder.DropTable(
                name: "survey_assignment");

            migrationBuilder.DropTable(
                name: "investigation");

            migrationBuilder.DropTable(
                name: "claim_master");
        }
    }
}

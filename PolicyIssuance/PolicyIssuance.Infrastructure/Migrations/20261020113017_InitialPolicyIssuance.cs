using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolicyIssuance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPolicyIssuance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    policy_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    quote_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    underwriting_id = table.Column<long>(type: "bigint", nullable: false),
                    issue_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    effective_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    premium_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    policy_status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.policy_id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyCoverages",
                columns: table => new
                {
                    coverage_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    coverage_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum_insured = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    coverage_start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    coverage_end_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyCoverages", x => x.coverage_id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyDocuments",
                columns: table => new
                {
                    policy_document_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    document_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    document_path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    generated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyDocuments", x => x.policy_document_id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyEndorsements",
                columns: table => new
                {
                    endorsement_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    endorsement_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    endorsement_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    old_values = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    new_values = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    approved_by = table.Column<long>(type: "bigint", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    approved_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyEndorsements", x => x.endorsement_id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyHistories",
                columns: table => new
                {
                    policy_history_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    performed_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    performed_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyHistories", x => x.policy_history_id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyNominees",
                columns: table => new
                {
                    policy_nominee_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    nominee_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    share_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyNominees", x => x.policy_nominee_id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRiders",
                columns: table => new
                {
                    rider_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    rider_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    premium_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRiders", x => x.rider_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "PolicyCoverages");

            migrationBuilder.DropTable(
                name: "PolicyDocuments");

            migrationBuilder.DropTable(
                name: "PolicyEndorsements");

            migrationBuilder.DropTable(
                name: "PolicyHistories");

            migrationBuilder.DropTable(
                name: "PolicyNominees");

            migrationBuilder.DropTable(
                name: "PolicyRiders");
        }
    }
}

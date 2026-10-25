using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Underwriting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "financial_underwriting",
                columns: table => new
                {
                    financial_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    underwriting_id = table.Column<int>(type: "int", nullable: false),
                    annual_income = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    employer_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    occupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    existing_insurance = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    loan_liability = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_underwriting", x => x.financial_id);
                });

            migrationBuilder.CreateTable(
                name: "medical_underwriting",
                columns: table => new
                {
                    medical_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    underwriting_id = table.Column<int>(type: "int", nullable: false),
                    height = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    bmi = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    smoking = table.Column<bool>(type: "bit", nullable: false),
                    alcohol = table.Column<bool>(type: "bit", nullable: false),
                    medical_history = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    medical_report_path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medical_underwriting", x => x.medical_id);
                });

            migrationBuilder.CreateTable(
                name: "risk_score",
                columns: table => new
                {
                    risk_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    underwriting_id = table.Column<int>(type: "int", nullable: false),
                    age_score = table.Column<int>(type: "int", nullable: false),
                    medical_score = table.Column<int>(type: "int", nullable: false),
                    occupation_score = table.Column<int>(type: "int", nullable: false),
                    lifestyle_score = table.Column<int>(type: "int", nullable: false),
                    financial_score = table.Column<int>(type: "int", nullable: false),
                    total_score = table.Column<int>(type: "int", nullable: false),
                    risk_category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_score", x => x.risk_id);
                });

            migrationBuilder.CreateTable(
                name: "underwriting_audit_log",
                columns: table => new
                {
                    audit_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    underwriting_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "varchar(100)", nullable: true),
                    performed_by = table.Column<int>(type: "int", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    action_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_underwriting_audit_log", x => x.audit_id);
                });

            migrationBuilder.CreateTable(
                name: "underwriting_case",
                columns: table => new
                {
                    underwriting_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quote_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    underwriting_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_underwriting_case", x => x.underwriting_id);
                });

            migrationBuilder.CreateTable(
                name: "underwriting_decision",
                columns: table => new
                {
                    decision_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    underwriting_id = table.Column<int>(type: "int", nullable: false),
                    decision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    loading_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    exclusion_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rejection_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approved_by = table.Column<int>(type: "int", nullable: false),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_underwriting_decision", x => x.decision_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "financial_underwriting");

            migrationBuilder.DropTable(
                name: "medical_underwriting");

            migrationBuilder.DropTable(
                name: "risk_score");

            migrationBuilder.DropTable(
                name: "underwriting_audit_log");

            migrationBuilder.DropTable(
                name: "underwriting_case");

            migrationBuilder.DropTable(
                name: "underwriting_decision");
        }
    }
}

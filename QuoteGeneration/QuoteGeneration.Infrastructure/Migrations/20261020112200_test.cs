using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuoteGeneration.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "discount_master",
                columns: table => new
                {
                    discount_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    discount_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    valid_from = table.Column<DateTime>(type: "datetime2", nullable: true),
                    valid_to = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discount_master", x => x.discount_id);
                });

            migrationBuilder.CreateTable(
                name: "quote",
                columns: table => new
                {
                    quote_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quote_number = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    policy_term = table.Column<int>(type: "int", nullable: false),
                    sum_insured = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    premium_frequency_id = table.Column<int>(type: "int", nullable: true),
                    quote_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    quote_status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quote", x => x.quote_id);
                });

            migrationBuilder.CreateTable(
                name: "RiderMaster",
                columns: table => new
                {
                    rider_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    rider_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    rider_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rider_premium = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiderMaster", x => x.rider_id);
                });

            migrationBuilder.CreateTable(
                name: "premium_calculation",
                columns: table => new
                {
                    calculation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quote_id = table.Column<int>(type: "int", nullable: false),
                    base_premium = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    risk_loading = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    rider_premium = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    discount_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    gst_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    final_premium = table.Column<decimal>(type: "decimal(15,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_premium_calculation", x => x.calculation_id);
                    table.ForeignKey(
                        name: "FK_premium_calculation_quote_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quote",
                        principalColumn: "quote_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quote_history",
                columns: table => new
                {
                    history_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quote_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    performed_by = table.Column<int>(type: "int", nullable: true),
                    action_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quote_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_quote_history_quote_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quote",
                        principalColumn: "quote_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "risk_assessment",
                columns: table => new
                {
                    risk_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quote_id = table.Column<int>(type: "int", nullable: false),
                    age_score = table.Column<int>(type: "int", nullable: true),
                    occupation_score = table.Column<int>(type: "int", nullable: true),
                    medical_score = table.Column<int>(type: "int", nullable: true),
                    lifestyle_score = table.Column<int>(type: "int", nullable: true),
                    total_score = table.Column<int>(type: "int", nullable: true),
                    risk_category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_assessment", x => x.risk_id);
                    table.ForeignKey(
                        name: "FK_risk_assessment_quote_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quote",
                        principalColumn: "quote_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quote_rider",
                columns: table => new
                {
                    rider_selection_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quote_id = table.Column<int>(type: "int", nullable: false),
                    rider_id = table.Column<int>(type: "int", nullable: false),
                    rider_premium = table.Column<decimal>(type: "decimal(15,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quote_rider", x => x.rider_selection_id);
                    table.ForeignKey(
                        name: "FK_quote_rider_RiderMaster_rider_id",
                        column: x => x.rider_id,
                        principalTable: "RiderMaster",
                        principalColumn: "rider_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quote_rider_quote_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quote",
                        principalColumn: "quote_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_premium_calculation_quote_id",
                table: "premium_calculation",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_history_quote_id",
                table: "quote_history",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_rider_quote_id",
                table: "quote_rider",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_rider_rider_id",
                table: "quote_rider",
                column: "rider_id");

            migrationBuilder.CreateIndex(
                name: "IX_risk_assessment_quote_id",
                table: "risk_assessment",
                column: "quote_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "discount_master");

            migrationBuilder.DropTable(
                name: "premium_calculation");

            migrationBuilder.DropTable(
                name: "quote_history");

            migrationBuilder.DropTable(
                name: "quote_rider");

            migrationBuilder.DropTable(
                name: "risk_assessment");

            migrationBuilder.DropTable(
                name: "RiderMaster");

            migrationBuilder.DropTable(
                name: "quote");
        }
    }
}

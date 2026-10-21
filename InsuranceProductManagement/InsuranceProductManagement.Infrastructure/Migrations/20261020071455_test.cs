using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceProductManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "premium_frequency",
                columns: table => new
                {
                    frequency_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    frequency_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    installment_count = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_premium_frequency", x => x.frequency_id);
                });

            migrationBuilder.CreateTable(
                name: "product_category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "insurance_product",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    product_code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    product_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    minimum_age = table.Column<int>(type: "int", nullable: true),
                    maximum_age = table.Column<int>(type: "int", nullable: true),
                    minimum_sum_insured = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    maximum_sum_insured = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_insurance_product", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_insurance_product_product_category_category_id",
                        column: x => x.category_id,
                        principalTable: "product_category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "benefit_master",
                columns: table => new
                {
                    benefit_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    benefit_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    benefit_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    benefit_limit = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_benefit_master", x => x.benefit_id);
                    table.ForeignKey(
                        name: "FK_benefit_master_insurance_product_product_id",
                        column: x => x.product_id,
                        principalTable: "insurance_product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "coverage_master",
                columns: table => new
                {
                    coverage_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    coverage_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    minimum_coverage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    maximum_coverage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    waiting_period_days = table.Column<int>(type: "int", nullable: true),
                    deductible_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coverage_master", x => x.coverage_id);
                    table.ForeignKey(
                        name: "FK_coverage_master_insurance_product_product_id",
                        column: x => x.product_id,
                        principalTable: "insurance_product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_commission",
                columns: table => new
                {
                    commission_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    first_year_commission = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    renewal_commission = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    branch_commission = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_commission", x => x.commission_id);
                    table.ForeignKey(
                        name: "FK_product_commission_insurance_product_product_id",
                        column: x => x.product_id,
                        principalTable: "insurance_product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rider_master",
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
                    table.PrimaryKey("PK_rider_master", x => x.rider_id);
                    table.ForeignKey(
                        name: "FK_rider_master_insurance_product_product_id",
                        column: x => x.product_id,
                        principalTable: "insurance_product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_benefit_master_product_id",
                table: "benefit_master",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_coverage_master_product_id",
                table: "coverage_master",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_insurance_product_category_id",
                table: "insurance_product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_commission_product_id",
                table: "product_commission",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_rider_master_product_id",
                table: "rider_master",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "benefit_master");

            migrationBuilder.DropTable(
                name: "coverage_master");

            migrationBuilder.DropTable(
                name: "premium_frequency");

            migrationBuilder.DropTable(
                name: "product_commission");

            migrationBuilder.DropTable(
                name: "rider_master");

            migrationBuilder.DropTable(
                name: "insurance_product");

            migrationBuilder.DropTable(
                name: "product_category");
        }
    }
}

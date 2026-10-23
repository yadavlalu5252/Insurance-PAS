using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "claim_settlement",
                columns: table => new
                {
                    settlement_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    approved_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    payment_mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transaction_reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    settlement_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    settlement_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    account_holder_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    account_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ifsc_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bank_verified = table.Column<bool>(type: "bit", nullable: false),
                    processed_by = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_settlement", x => x.settlement_id);
                    table.ForeignKey(
                        name: "FK_claim_settlement_claim_master_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claim_master",
                        principalColumn: "claim_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_claim_settlement_claim_id",
                table: "claim_settlement",
                column: "claim_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claim_settlement");
        }
    }
}

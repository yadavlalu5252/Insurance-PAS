using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolicyIssuance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PolicyClosure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "approved_by",
                table: "PolicyEndorsements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PolicyClosures",
                columns: table => new
                {
                    policy_closure_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    closure_reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requested_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requested_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    closure_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    approved_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    rejection_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    closed_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    refund_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyClosures", x => x.policy_closure_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyClosures");

            migrationBuilder.AlterColumn<long>(
                name: "approved_by",
                table: "PolicyEndorsements",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremiumSchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentReceipts",
                columns: table => new
                {
                    receipt_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payment_id = table.Column<int>(type: "int", nullable: false),
                    receipt_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    receipt_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    receipt_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReceipts", x => x.receipt_id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumPayments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    schedule_id = table.Column<int>(type: "int", nullable: false),
                    policy_id = table.Column<int>(type: "int", nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    amount_paid = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    payment_mode = table.Column<int>(type: "int", nullable: false),
                    transaction_reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    payment_status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumPayments", x => x.payment_id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumRefunds",
                columns: table => new
                {
                    refund_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payment_id = table.Column<int>(type: "int", nullable: false),
                    refund_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    refund_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    refund_status = table.Column<int>(type: "int", nullable: false),
                    approved_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumRefunds", x => x.refund_id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumReminders",
                columns: table => new
                {
                    reminder_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    schedule_id = table.Column<long>(type: "bigint", nullable: false),
                    reminder_type = table.Column<int>(type: "int", nullable: false),
                    reminder_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delivery_status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumReminders", x => x.reminder_id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumSchedule",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<int>(type: "int", nullable: false),
                    installment_no = table.Column<int>(type: "int", nullable: false),
                    due_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    premium_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    late_fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumSchedule", x => x.schedule_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentReceipts");

            migrationBuilder.DropTable(
                name: "PremiumPayments");

            migrationBuilder.DropTable(
                name: "PremiumRefunds");

            migrationBuilder.DropTable(
                name: "PremiumReminders");

            migrationBuilder.DropTable(
                name: "PremiumSchedule");
        }
    }
}

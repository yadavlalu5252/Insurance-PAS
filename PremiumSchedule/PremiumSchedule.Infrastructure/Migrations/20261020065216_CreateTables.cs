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
                name: "PremiumSchedule",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<int>(type: "int", nullable: false),
                    premium_frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    installment_number = table.Column<int>(type: "int", nullable: false),
                    due_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    premium_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                name: "PremiumSchedule");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investigation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "investigation",
                columns: table => new
                {
                    investigation_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    investigator_id = table.Column<long>(type: "bigint", nullable: false),
                    investigation_reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    report_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    investigation_status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investigation", x => x.investigation_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "investigation");
        }
    }
}

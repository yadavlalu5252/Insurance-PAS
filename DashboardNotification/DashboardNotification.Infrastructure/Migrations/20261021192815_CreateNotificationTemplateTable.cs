using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DashboardNotification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateNotificationTemplateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification_template",
                columns: table => new
                {
                    template_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    template_name = table.Column<string>(type: "varchar(150)", nullable: true),
                    notification_type = table.Column<string>(type: "varchar(20)", nullable: true),
                    subject = table.Column<string>(type: "varchar(255)", nullable: true),
                    body = table.Column<string>(type: "varchar(max)", nullable: true),
                    version_no = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_template", x => x.template_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_template");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolicyIssuance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPolicyRiderPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PolicyRiders",
                table: "PolicyRiders");

            migrationBuilder.AlterColumn<long>(
                name: "rider_id",
                table: "PolicyRiders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "policy_rider_id",
                table: "PolicyRiders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PolicyRiders",
                table: "PolicyRiders",
                column: "policy_rider_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PolicyRiders",
                table: "PolicyRiders");

            migrationBuilder.DropColumn(
                name: "policy_rider_id",
                table: "PolicyRiders");

            migrationBuilder.AlterColumn<long>(
                name: "rider_id",
                table: "PolicyRiders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PolicyRiders",
                table: "PolicyRiders",
                column: "rider_id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolicyServicing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedHistoryShadowProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_service_request_history_service_request_Service_Requestrequest_id",
            //    table: "service_request_history");

            //migrationBuilder.DropIndex(
            //    name: "IX_service_request_history_Service_Requestrequest_id",
            //    table: "service_request_history");

            //migrationBuilder.DropColumn(
            //    name: "Service_Requestrequest_id",
            //    table: "service_request_history");

            //migrationBuilder.CreateIndex(
            //    name: "IX_service_request_history_request_id",
            //    table: "service_request_history",
            //    column: "request_id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_service_request_history_service_request_request_id",
            //    table: "service_request_history",
            //    column: "request_id",
            //    principalTable: "service_request",
            //    principalColumn: "request_id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_service_request_history_service_request_request_id",
                table: "service_request_history");

            migrationBuilder.DropIndex(
                name: "IX_service_request_history_request_id",
                table: "service_request_history");

            migrationBuilder.AddColumn<long>(
                name: "Service_Requestrequest_id",
                table: "service_request_history",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_service_request_history_Service_Requestrequest_id",
                table: "service_request_history",
                column: "Service_Requestrequest_id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_request_history_service_request_Service_Requestrequest_id",
                table: "service_request_history",
                column: "Service_Requestrequest_id",
                principalTable: "service_request",
                principalColumn: "request_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

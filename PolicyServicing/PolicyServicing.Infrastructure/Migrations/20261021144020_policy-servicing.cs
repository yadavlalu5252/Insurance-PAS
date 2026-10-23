using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolicyServicing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class policyservicing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nominee_details",
                columns: table => new
                {
                    nominee_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    nominee_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dob = table.Column<DateOnly>(type: "date", nullable: false),
                    share_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    guardian_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nominee_details", x => x.nominee_id);
                });

            migrationBuilder.CreateTable(
                name: "policy_renewal",
                columns: table => new
                {
                    renewal_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    renewal_date = table.Column<DateOnly>(type: "date", nullable: false),
                    new_expiry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    premium_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Renewal_Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_policy_renewal", x => x.renewal_id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyCancellations",
                columns: table => new
                {
                    cancellation_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    cancellation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    cancellation_reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    refund_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    approved_by = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyCancellations", x => x.cancellation_id);
                });

            migrationBuilder.CreateTable(
                name: "service_request",
                columns: table => new
                {
                    request_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    request_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    request_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_request", x => x.request_id);
                });

            migrationBuilder.CreateTable(
                name: "policy_endorsements",
                columns: table => new
                {
                    endorsement_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_id = table.Column<long>(type: "bigint", nullable: false),
                    endorsement_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    endorsement_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    previous_value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    approval_status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_policy_endorsements", x => x.endorsement_id);
                    table.ForeignKey(
                        name: "FK_policy_endorsements_service_request_request_id",
                        column: x => x.request_id,
                        principalTable: "service_request",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_request_history",
                columns: table => new
                {
                    history_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_id = table.Column<long>(type: "bigint", nullable: false),
                    Service_Requestrequest_id = table.Column<long>(type: "bigint", nullable: false),
                    action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    action_by = table.Column<long>(type: "bigint", nullable: false),
                    action_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_request_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_service_request_history_service_request_Service_Requestrequest_id",
                        column: x => x.Service_Requestrequest_id,
                        principalTable: "service_request",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_policy_endorsements_request_id",
                table: "policy_endorsements",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_history_Service_Requestrequest_id",
                table: "service_request_history",
                column: "Service_Requestrequest_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nominee_details");

            migrationBuilder.DropTable(
                name: "policy_endorsements");

            migrationBuilder.DropTable(
                name: "policy_renewal");

            migrationBuilder.DropTable(
                name: "PolicyCancellations");

            migrationBuilder.DropTable(
                name: "service_request_history");

            migrationBuilder.DropTable(
                name: "service_request");
        }
    }
}

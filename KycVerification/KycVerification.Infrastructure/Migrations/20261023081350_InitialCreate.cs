using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KycVerification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ckyc_details",
                columns: table => new
                {
                    ckyc_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    ckyc_number = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    verification_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ckyc_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ckyc_details", x => x.ckyc_id);
                });

            migrationBuilder.CreateTable(
                name: "customer_kyc",
                columns: table => new
                {
                    kyc_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    kyc_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    kyc_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    verification_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    verified_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    verified_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_kyc", x => x.kyc_id);
                });

            migrationBuilder.CreateTable(
                name: "document_type_master",
                columns: table => new
                {
                    document_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    document_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    mandatory_flag = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_type_master", x => x.document_type_id);
                });

            migrationBuilder.CreateTable(
                name: "kyc_document",
                columns: table => new
                {
                    document_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kyc_id = table.Column<int>(type: "int", nullable: false),
                    document_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    document_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    file_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    upload_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    verification_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kyc_document", x => x.document_id);
                });

            migrationBuilder.CreateTable(
                name: "kyc_verification_log",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kyc_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    performed_by = table.Column<int>(type: "int", nullable: true),
                    action_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kyc_verification_log", x => x.log_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ckyc_details");

            migrationBuilder.DropTable(
                name: "customer_kyc");

            migrationBuilder.DropTable(
                name: "document_type_master");

            migrationBuilder.DropTable(
                name: "kyc_document");

            migrationBuilder.DropTable(
                name: "kyc_verification_log");
        }
    }
}

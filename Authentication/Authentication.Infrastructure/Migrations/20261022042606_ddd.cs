using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "two_factor_enabled",
                table: "user_master",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "two_factor_last_used_step",
                table: "user_master",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "two_factor_secret",
                table: "user_master",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "two_factor_login_challenges",
                columns: table => new
                {
                    challenge_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    challenge_token_hash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    failed_attempts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_two_factor_login_challenges", x => x.challenge_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_two_factor_login_challenges_challenge_token_hash",
                table: "two_factor_login_challenges",
                column: "challenge_token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_two_factor_login_challenges_user_id",
                table: "two_factor_login_challenges",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "two_factor_login_challenges");

            migrationBuilder.DropColumn(
                name: "two_factor_enabled",
                table: "user_master");

            migrationBuilder.DropColumn(
                name: "two_factor_last_used_step",
                table: "user_master");

            migrationBuilder.DropColumn(
                name: "two_factor_secret",
                table: "user_master");
        }
    }
}

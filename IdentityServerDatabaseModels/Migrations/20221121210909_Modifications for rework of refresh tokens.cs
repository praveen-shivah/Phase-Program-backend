using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServerDatabaseModels.Migrations
{
    public partial class Modificationsforreworkofrefreshtokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRefreshToken",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "Audience",
                table: "RefreshToken",
                type: "character(200)",
                fixedLength: true,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Issuer",
                table: "RefreshToken",
                type: "character(200)",
                fixedLength: true,
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Issuer_Audience_ReplacedByToken",
                table: "RefreshToken",
                columns: new[] { "Issuer", "Audience", "ReplacedByToken" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_Issuer_Audience_ReplacedByToken",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "Audience",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "Issuer",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "CurrentRefreshToken",
                table: "Account",
                type: "character(500)",
                fixedLength: true,
                maxLength: 500,
                nullable: true);
        }
    }
}

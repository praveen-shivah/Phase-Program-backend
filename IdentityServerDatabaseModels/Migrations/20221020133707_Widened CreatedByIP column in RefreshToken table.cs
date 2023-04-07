using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServerDatabaseModels.Migrations
{
    public partial class WidenedCreatedByIPcolumninRefreshTokentable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                table: "RefreshToken",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(20)",
                oldFixedLength: true,
                oldMaxLength: 20);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                table: "RefreshToken",
                type: "character(20)",
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}

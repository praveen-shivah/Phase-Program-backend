using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServerDatabaseModels.Migrations
{
    public partial class ChangedRolestoClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Roles",
                table: "Account",
                newName: "Claims");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Claims",
                table: "Account",
                newName: "Roles");
        }
    }
}

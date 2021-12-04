using Microsoft.EntityFrameworkCore.Migrations;

namespace authservice.Migrations
{
    public partial class RenameUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "user_table");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_table",
                table: "user_table",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_table",
                table: "user_table");

            migrationBuilder.RenameTable(
                name: "user_table",
                newName: "User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "id");
        }
    }
}

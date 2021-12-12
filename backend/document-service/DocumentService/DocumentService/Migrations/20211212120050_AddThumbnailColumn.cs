using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentService.Migrations
{
    public partial class AddThumbnailColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "thumbnail",
                table: "document_table",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnail",
                table: "document_table");
        }
    }
}

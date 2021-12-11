using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "directory_table",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    parent_directory_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_directory_table", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "document_table",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    directory_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    s3_location = table.Column<string>(type: "text", nullable: true),
                    file_size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_table", x => x.id);
                    table.ForeignKey(
                        name: "FK_document_table_directory_table_directory_id",
                        column: x => x.directory_id,
                        principalTable: "directory_table",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_document_table_directory_id",
                table: "document_table",
                column: "directory_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document_table");

            migrationBuilder.DropTable(
                name: "directory_table");
        }
    }
}

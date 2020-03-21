using Microsoft.EntityFrameworkCore.Migrations;

namespace YukiDrive.Migrations.Site
{
    public partial class AddPropHiddenFolders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HiddenFolders",
                table: "Sites",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HiddenFolders",
                table: "Sites");
        }
    }
}

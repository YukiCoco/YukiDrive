using Microsoft.EntityFrameworkCore.Migrations;

namespace YukiDrive.Migrations.Site
{
    public partial class HiddenDrectory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HiddenDrectory",
                table: "Sites",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HiddenDrectory",
                table: "Sites");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace YukiDrive.Migrations.Site
{
    public partial class AddSiteNickName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NickName",
                table: "Sites",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NickName",
                table: "Sites");
        }
    }
}

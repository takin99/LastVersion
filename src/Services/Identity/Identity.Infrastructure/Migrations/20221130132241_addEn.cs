using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sattec.Identity.Infrastructure.Migrations
{
    public partial class addEn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassWord",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmPassWord",
                table: "AspNetUsers");
        }
    }
}

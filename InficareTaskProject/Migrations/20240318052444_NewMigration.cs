using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InficareTaskProject.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RollNo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RollNo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "AspNetUsers");
        }
    }
}

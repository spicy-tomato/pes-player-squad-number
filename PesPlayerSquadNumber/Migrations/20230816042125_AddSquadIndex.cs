using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesPlayerSquadNumber.Migrations
{
    public partial class AddSquadIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SquadIndex",
                table: "Players",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SquadIndex",
                table: "Players");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class migr2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "rowNumber",
                table: "EnergyTable",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnergyTable",
                table: "EnergyTable",
                columns: new[] { "rowNumber", "MeterID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EnergyTable",
                table: "EnergyTable");

            migrationBuilder.DropColumn(
                name: "rowNumber",
                table: "EnergyTable");
        }
    }
}

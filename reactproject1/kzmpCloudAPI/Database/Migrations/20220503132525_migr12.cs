using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class migr12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "recordCreateDate",
                table: "reportsHistory",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "time",
                table: "power_profile_m",
                type: "time",
                nullable: false,
                oldClrType: typeof(object),
                oldType: "sql_variant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "recordCreateDate",
                table: "reportsHistory");

            migrationBuilder.AlterColumn<object>(
                name: "time",
                table: "power_profile_m",
                type: "sql_variant",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }
    }
}

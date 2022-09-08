using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class migr14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionInfo",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SessionToken = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    DeviceId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    AuthTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionInfo", x => new { x.UserId, x.DeviceId, x.SessionToken });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionInfo");
        }
    }
}

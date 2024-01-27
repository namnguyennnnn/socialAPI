using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn4.Migrations
{
    public partial class alaaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SenderId",
                table: "Notifies",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Notifies");
        }
    }
}

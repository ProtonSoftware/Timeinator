using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Timeinator.Mobile.DataAccess.Migrations
{
    public partial class AssignedTimeOnTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "AssignedTime",
                table: "TimeTasks",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTime",
                table: "TimeTasks");
        }
    }
}

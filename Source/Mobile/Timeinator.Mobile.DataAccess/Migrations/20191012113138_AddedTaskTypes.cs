using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Timeinator.Mobile.DataAccess.Migrations
{
    public partial class AddedTaskTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MaxProgress",
                table: "TimeTasks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TimeTasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxProgress",
                table: "TimeTasks");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TimeTasks");
        }
    }
}

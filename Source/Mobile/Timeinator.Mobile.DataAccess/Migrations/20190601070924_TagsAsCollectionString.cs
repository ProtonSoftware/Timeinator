using Microsoft.EntityFrameworkCore.Migrations;

namespace Timeinator.Mobile.DataAccess.Migrations
{
    public partial class TagsAsCollectionString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "TimeTasks",
                newName: "TagsString");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TagsString",
                table: "TimeTasks",
                newName: "Tag");
        }
    }
}

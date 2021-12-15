using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectTracker.Data.Migrations
{
    public partial class addSubmitterColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Submitter",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Submitter",
                table: "Tickets");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class MovieTheaters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieTheater",
                table: "MovieTheater");

            migrationBuilder.RenameTable(
                name: "MovieTheater",
                newName: "MovieTheaters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieTheaters",
                table: "MovieTheaters",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieTheaters",
                table: "MovieTheaters");

            migrationBuilder.RenameTable(
                name: "MovieTheaters",
                newName: "MovieTheater");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieTheater",
                table: "MovieTheater",
                column: "Id");
        }
    }
}

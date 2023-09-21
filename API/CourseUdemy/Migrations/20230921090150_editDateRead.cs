using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseUdemy.Migrations
{
    /// <inheritdoc />
    public partial class editDateRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateRed",
                table: "Messages",
                newName: "DateRead");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateRead",
                table: "Messages",
                newName: "DateRed");
        }
    }
}

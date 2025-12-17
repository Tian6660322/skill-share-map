using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace skill_share_map.Migrations
{
    /// <inheritdoc />
    public partial class AddDegree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Degree",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                table: "Users");
        }
    }
}

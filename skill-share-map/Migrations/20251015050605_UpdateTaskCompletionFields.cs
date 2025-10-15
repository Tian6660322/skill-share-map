using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace skill_share_map.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskCompletionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatorConfirmedAt",
                table: "SkillTasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HelperConfirmedAt",
                table: "SkillTasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatorConfirmedComplete",
                table: "SkillTasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHelperConfirmedComplete",
                table: "SkillTasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorConfirmedAt",
                table: "SkillTasks");

            migrationBuilder.DropColumn(
                name: "HelperConfirmedAt",
                table: "SkillTasks");

            migrationBuilder.DropColumn(
                name: "IsCreatorConfirmedComplete",
                table: "SkillTasks");

            migrationBuilder.DropColumn(
                name: "IsHelperConfirmedComplete",
                table: "SkillTasks");
        }
    }
}

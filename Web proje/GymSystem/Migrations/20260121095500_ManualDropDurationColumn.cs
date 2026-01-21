using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymSystem.Migrations
{
    /// <inheritdoc />
    public partial class ManualDropDurationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInWeeks",
                table: "Programs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInWeeks",
                table: "Programs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

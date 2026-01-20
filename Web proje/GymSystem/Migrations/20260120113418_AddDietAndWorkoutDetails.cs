using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDietAndWorkoutDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Programs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DietPlanId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DietPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietPlans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_DietPlanId",
                table: "Members",
                column: "DietPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_DietPlans_DietPlanId",
                table: "Members",
                column: "DietPlanId",
                principalTable: "DietPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_DietPlans_DietPlanId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "DietPlans");

            migrationBuilder.DropIndex(
                name: "IX_Members_DietPlanId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "DietPlanId",
                table: "Members");
        }
    }
}

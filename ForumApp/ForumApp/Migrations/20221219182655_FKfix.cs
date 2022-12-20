using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumApp.Migrations
{
    public partial class FKfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Subforums_SubforumId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Subforums_Forums_ForumId",
                table: "Subforums");

            migrationBuilder.AlterColumn<int>(
                name: "ForumId",
                table: "Subforums",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubforumId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Subforums_SubforumId",
                table: "Posts",
                column: "SubforumId",
                principalTable: "Subforums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subforums_Forums_ForumId",
                table: "Subforums",
                column: "ForumId",
                principalTable: "Forums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Subforums_SubforumId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Subforums_Forums_ForumId",
                table: "Subforums");

            migrationBuilder.AlterColumn<int>(
                name: "ForumId",
                table: "Subforums",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SubforumId",
                table: "Posts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Subforums_SubforumId",
                table: "Posts",
                column: "SubforumId",
                principalTable: "Subforums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subforums_Forums_ForumId",
                table: "Subforums",
                column: "ForumId",
                principalTable: "Forums",
                principalColumn: "Id");
        }
    }
}

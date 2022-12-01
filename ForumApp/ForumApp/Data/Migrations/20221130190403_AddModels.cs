using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumApp.Data.Migrations
{
    public partial class AddModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountOfForums = table.Column<int>(type: "int", nullable: false),
                    SectionType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    ForumName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ForumType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountOfSubforums = table.Column<int>(type: "int", nullable: false),
                    MsgCount = table.Column<int>(type: "int", nullable: false),
                    ForumDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forums_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subforums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ForumId = table.Column<int>(type: "int", nullable: false),
                    SubforumType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubforumName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MsgCount = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    Creator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastPostUsr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subforums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subforums_Forums_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostContent = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    SubforumId = table.Column<int>(type: "int", nullable: false),
                    PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Subforums_SubforumId",
                        column: x => x.SubforumId,
                        principalTable: "Subforums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Forums_SectionId",
                table: "Forums",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SubforumId",
                table: "Posts",
                column: "SubforumId");

            migrationBuilder.CreateIndex(
                name: "IX_Subforums_ForumId",
                table: "Subforums",
                column: "ForumId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Subforums");

            migrationBuilder.DropTable(
                name: "Forums");

            migrationBuilder.DropTable(
                name: "Sections");
        }
    }
}

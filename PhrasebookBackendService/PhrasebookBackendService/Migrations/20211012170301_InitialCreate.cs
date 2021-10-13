using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhrasebookBackendService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Phrasebooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstLanguageId = table.Column<int>(type: "int", nullable: true),
                    ForeignLanguageId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phrasebooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phrasebooks_Languages_FirstLanguageId",
                        column: x => x.FirstLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phrasebooks_Languages_ForeignLanguageId",
                        column: x => x.ForeignLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phrasebooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhrasebookId = table.Column<int>(type: "int", nullable: false),
                    FirstLanguagePhrase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ForeignLanguagePhrase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LexicalItemType = table.Column<int>(type: "int", nullable: false),
                    ForeignLanguageSynonyms = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrectnessCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phrases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phrases_Phrasebooks_PhrasebookId",
                        column: x => x.PhrasebookId,
                        principalTable: "Phrasebooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phrasebooks_FirstLanguageId",
                table: "Phrasebooks",
                column: "FirstLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrasebooks_ForeignLanguageId",
                table: "Phrasebooks",
                column: "ForeignLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrasebooks_UserId",
                table: "Phrasebooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrases_PhrasebookId",
                table: "Phrases",
                column: "PhrasebookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Phrases");

            migrationBuilder.DropTable(
                name: "Phrasebooks");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

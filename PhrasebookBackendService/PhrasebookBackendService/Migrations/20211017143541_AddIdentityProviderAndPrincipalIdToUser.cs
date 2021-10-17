using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhrasebookBackendService.Migrations
{
    public partial class AddIdentityProviderAndPrincipalIdToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityProvider",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrincipalId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrincipalId",
                table: "Users",
                column: "PrincipalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PrincipalId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IdentityProvider",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrincipalId",
                table: "Users");
        }
    }
}

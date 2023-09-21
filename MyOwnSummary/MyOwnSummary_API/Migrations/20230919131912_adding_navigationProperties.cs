using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyOwnSummaryAPI.Migrations
{
    /// <inheritdoc />
    public partial class addingnavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLanguage_Language_LanguageId",
                table: "UserLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLanguage_User_UserId",
                table: "UserLanguage");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLanguage_Language_LanguageId",
                table: "UserLanguage",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLanguage_User_UserId",
                table: "UserLanguage",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLanguage_Language_LanguageId",
                table: "UserLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLanguage_User_UserId",
                table: "UserLanguage");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLanguage_Language_LanguageId",
                table: "UserLanguage",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLanguage_User_UserId",
                table: "UserLanguage",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

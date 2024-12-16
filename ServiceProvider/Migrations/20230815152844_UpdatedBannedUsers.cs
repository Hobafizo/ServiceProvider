using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceProvider.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBannedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers",
                column: "UserId",
                unique: true);
        }
    }
}

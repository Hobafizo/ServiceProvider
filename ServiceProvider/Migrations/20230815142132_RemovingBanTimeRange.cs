using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceProvider.Migrations
{
    /// <inheritdoc />
    public partial class RemovingBanTimeRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeginTime",
                table: "BannedUsers");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "BannedUsers",
                newName: "BanTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BanTime",
                table: "BannedUsers",
                newName: "EndTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "BeginTime",
                table: "BannedUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class Addednewfieldstofilemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_UserId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Files",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_UserId",
                table: "Files",
                newName: "IX_Files_OwnerId");

            migrationBuilder.AddColumn<int>(
                name: "Downloads",
                table: "Files",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "Files",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_OwnerId",
                table: "Files",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_OwnerId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Downloads",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Files",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_OwnerId",
                table: "Files",
                newName: "IX_Files_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_UserId",
                table: "Files",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

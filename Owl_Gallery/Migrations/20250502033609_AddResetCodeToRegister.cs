using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Owl_Gallery.Migrations
{
    /// <inheritdoc />
    public partial class AddResetCodeToRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeSentAt",
                table: "Registers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetCode",
                table: "Registers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeSentAt",
                table: "Registers");

            migrationBuilder.DropColumn(
                name: "ResetCode",
                table: "Registers");
        }
    }
}

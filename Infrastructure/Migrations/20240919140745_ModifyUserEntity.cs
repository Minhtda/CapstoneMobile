using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBuisnessAccount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsMailExisted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$.0w40YUYq4CHCYxQtqLGZu.9gOn54sErc8TKp9ZL082uisIzgvPzK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$H010Qci3CMkFYkx2EIYd1.bw9DWHSmknZt3rAhn6VsZjXZHREahEW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBuisnessAccount",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMailExisted",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                columns: new[] { "IsBuisnessAccount", "IsMailExisted", "PasswordHash", "Token" },
                values: new object[] { null, null, "$2a$11$fhkjd27qTXhL9a06CWyuSuqS4S6mPVW6ymkdKlPyDqQpRIYKwgVV.", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                columns: new[] { "IsBuisnessAccount", "IsMailExisted", "PasswordHash", "Token" },
                values: new object[] { null, null, "$2a$11$In9gwqu0k.OLKFdbi7ycY.aiPcmBgLPy7sCaylf8pZ.mgf/FPlr6.", null });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingFeeToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ShippingFee",
                table: "Orders",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$fhkjd27qTXhL9a06CWyuSuqS4S6mPVW6ymkdKlPyDqQpRIYKwgVV.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$In9gwqu0k.OLKFdbi7ycY.aiPcmBgLPy7sCaylf8pZ.mgf/FPlr6.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$65uHxz3izgaXGH3ccrufKOmmlYdgHhCsAQVyorH0UhygsaVDXeXPe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$5j0iq878zzmsHoF1pPm3H.NWkll5YR7gRUELC7f07EqVu9wm5Dz7K");
        }
    }
}

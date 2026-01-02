using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Password", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 3, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(3970), "", new DateTime(2025, 12, 3, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(3974) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Password", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 8, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(4020), "", new DateTime(2025, 12, 8, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(4020) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Password", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 13, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(4025), "", new DateTime(2025, 12, 13, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(4025) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "Password", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(4033), "", new DateTime(2025, 12, 18, 10, 57, 16, 740, DateTimeKind.Utc).AddTicks(4034) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7941), new DateTime(2025, 12, 3, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7945) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 8, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7950), new DateTime(2025, 12, 8, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7951) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 13, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7955), new DateTime(2025, 12, 13, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7955) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7959), new DateTime(2025, 12, 18, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7960) });
        }
    }
}

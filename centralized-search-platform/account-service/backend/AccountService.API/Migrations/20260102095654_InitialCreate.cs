using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccountService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "Email", "FirstName", "LastName", "PhoneNumber", "State", "Status", "UpdatedAt", "UserType", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "123 Seller Street", "New York", "USA", new DateTime(2025, 12, 3, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7941), "john.seller@example.com", "John", "Seller", "555-0101", "NY", 1, new DateTime(2025, 12, 3, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7945), 1, "10001" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "456 Buyer Avenue", "Los Angeles", "USA", new DateTime(2025, 12, 8, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7950), "jane.buyer@example.com", "Jane", "Buyer", "555-0102", "CA", 1, new DateTime(2025, 12, 8, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7951), 2, "90001" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "789 Carrier Road", "Chicago", "USA", new DateTime(2025, 12, 13, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7955), "mike.carrier@example.com", "Mike", "Carrier", "555-0103", "IL", 1, new DateTime(2025, 12, 13, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7955), 3, "60001" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "321 Agent Plaza", "Miami", "USA", new DateTime(2025, 12, 18, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7959), "sarah.agent@example.com", "Sarah", "Agent", "555-0104", "FL", 1, new DateTime(2025, 12, 18, 9, 56, 53, 744, DateTimeKind.Utc).AddTicks(7960), 4, "33101" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                table: "Users",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                table: "Users",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserType",
                table: "Users",
                column: "UserType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

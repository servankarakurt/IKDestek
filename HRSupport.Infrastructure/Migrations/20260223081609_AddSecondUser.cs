using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRSupport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSecondUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedTime", "Email", "IsDeleted", "IsPasswordChangeRequired", "Isactive", "PasswordHash", "Role" },
                values: new object[] { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "arkadaki@hepiyi.com", false, true, true, "$2a$11$BxM5sV7K9wL3pQ0rT4uN5eP6xQ7sR8uV9wX0yY1zZ2aA3bB4cC5d", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}

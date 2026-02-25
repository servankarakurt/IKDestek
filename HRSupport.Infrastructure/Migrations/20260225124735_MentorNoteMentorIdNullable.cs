using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRSupport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MentorNoteMentorIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MentorNotes_Employees_MentorId",
                table: "MentorNotes");

            migrationBuilder.AlterColumn<int>(
                name: "MentorId",
                table: "MentorNotes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MentorNotes_Employees_MentorId",
                table: "MentorNotes",
                column: "MentorId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MentorNotes_Employees_MentorId",
                table: "MentorNotes");

            migrationBuilder.AlterColumn<int>(
                name: "MentorId",
                table: "MentorNotes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorNotes_Employees_MentorId",
                table: "MentorNotes",
                column: "MentorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

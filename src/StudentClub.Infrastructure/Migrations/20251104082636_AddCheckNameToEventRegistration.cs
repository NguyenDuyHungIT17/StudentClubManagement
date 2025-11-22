using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentClub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckNameToEventRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckName",
                table: "EventRegistrations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckName",
                table: "EventRegistrations");
        }
    }
}

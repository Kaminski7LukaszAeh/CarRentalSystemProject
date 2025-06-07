using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addReturnCarLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "Reservations");
        }
    }
}

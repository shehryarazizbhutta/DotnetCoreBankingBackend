using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceAccountNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceAccountNumber",
                table: "Transactions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceAccountNumber",
                table: "Transactions");
        }
    }
}

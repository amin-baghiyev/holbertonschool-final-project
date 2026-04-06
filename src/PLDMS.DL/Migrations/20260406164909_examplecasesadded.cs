using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLDMS.DL.Migrations
{
    /// <inheritdoc />
    public partial class examplecasesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExample",
                table: "TestCases",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExample",
                table: "TestCases");
        }
    }
}

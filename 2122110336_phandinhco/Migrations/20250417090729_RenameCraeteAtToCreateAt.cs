using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2122110336_phandinhco.Migrations
{
    /// <inheritdoc />
    public partial class RenameCraeteAtToCreateAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "craeteAt",
                table: "categories",
                newName: "createAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "createAt",
                table: "categories",
                newName: "craeteAt");
        }

    }
}

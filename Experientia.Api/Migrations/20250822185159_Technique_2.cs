using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Experientia.Api.Migrations
{
    /// <inheritdoc />
    public partial class Technique_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Technique",
                table: "Technique");

            migrationBuilder.RenameTable(
                name: "Technique",
                newName: "Techniques");

            migrationBuilder.RenameIndex(
                name: "IX_Technique_Name",
                table: "Techniques",
                newName: "IX_Techniques_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Techniques",
                table: "Techniques",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Techniques",
                table: "Techniques");

            migrationBuilder.RenameTable(
                name: "Techniques",
                newName: "Technique");

            migrationBuilder.RenameIndex(
                name: "IX_Techniques_Name",
                table: "Technique",
                newName: "IX_Technique_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Technique",
                table: "Technique",
                column: "Id");
        }
    }
}

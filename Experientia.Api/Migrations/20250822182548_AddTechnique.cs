using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Experientia.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Technique",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technique", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Technique_Name",
                table: "Technique",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Technique");
        }
    }
}

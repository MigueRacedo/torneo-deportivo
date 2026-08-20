using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorneoDeportivo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CategoriaLlavesGeneradas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "llaves_generadas",
                table: "categorias",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "llaves_generadas",
                table: "categorias");
        }
    }
}

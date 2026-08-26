using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorneoDeportivo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioEscuela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "escuela",
                table: "usuarios",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "escuela",
                table: "usuarios");
        }
    }
}

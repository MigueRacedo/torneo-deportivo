using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorneoDeportivo.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Reemplaza el booleano <c>llaves_generadas</c> de las categorías por el enum <c>estado</c> del ciclo
    /// de vida (H0009), conservando la información existente.
    /// </summary>
    /// <remarks>
    /// Escrita a mano: la migración autogenerada hacía DropColumn + AddColumn con <c>defaultValue: ""</c>,
    /// lo que habría perdido qué categorías ya tenían bracket y habría dejado un valor que no parsea contra
    /// el enum. Acá se agrega la columna, se migran los datos y recién después se borra la vieja.
    /// El SQL directo es correcto en una migración (la regla de "nunca SQL fuera del repositorio" aplica al
    /// código de aplicación, no al versionado del esquema).
    /// </remarks>
    public partial class CategoriaEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "estado",
                table: "categorias",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "SinLlaves");

            // Las categorías que ya tenían bracket arrancan en LlavesGeneradas. No se puede inferir si
            // estaban EnCurso o Finalizada sin mirar los matches; el primer ganador que se registre las
            // recalcula, y en el peor caso el Coordinador ve una etapa atrasada, nunca datos perdidos.
            migrationBuilder.Sql(
                "UPDATE categorias SET estado = 'LlavesGeneradas' WHERE llaves_generadas = true;");

            migrationBuilder.DropColumn(
                name: "llaves_generadas",
                table: "categorias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "llaves_generadas",
                table: "categorias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Cualquier etapa distinta de SinLlaves implica que el bracket ya estaba armado.
            migrationBuilder.Sql(
                "UPDATE categorias SET llaves_generadas = true WHERE estado <> 'SinLlaves';");

            migrationBuilder.DropColumn(
                name: "estado",
                table: "categorias");
        }
    }
}

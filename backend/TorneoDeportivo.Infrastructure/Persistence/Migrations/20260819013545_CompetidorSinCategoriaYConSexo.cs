using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorneoDeportivo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompetidorSinCategoriaYConSexo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_competidores_categorias_categoria_id",
                table: "competidores");

            migrationBuilder.AlterColumn<decimal>(
                name: "peso",
                table: "competidores",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "categoria_id",
                table: "competidores",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "altura",
                table: "competidores",
                type: "numeric(4,2)",
                precision: 4,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            // Backfill para filas existentes con un valor válido del enum (M/F). Los nuevos inserts siempre lo proveen.
            migrationBuilder.AddColumn<string>(
                name: "sexo",
                table: "competidores",
                type: "text",
                nullable: false,
                defaultValue: "M");

            migrationBuilder.AddForeignKey(
                name: "fk_competidores_categorias_categoria_id",
                table: "competidores",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_competidores_categorias_categoria_id",
                table: "competidores");

            migrationBuilder.DropColumn(
                name: "sexo",
                table: "competidores");

            migrationBuilder.AlterColumn<decimal>(
                name: "peso",
                table: "competidores",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "categoria_id",
                table: "competidores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "altura",
                table: "competidores",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(4,2)",
                oldPrecision: 4,
                oldScale: 2);

            migrationBuilder.AddForeignKey(
                name: "fk_competidores_categorias_categoria_id",
                table: "competidores",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

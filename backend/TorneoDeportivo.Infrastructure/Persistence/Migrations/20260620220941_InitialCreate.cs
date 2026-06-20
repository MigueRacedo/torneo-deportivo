using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorneoDeportivo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "torneos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    lugar = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    imagen_flyer = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_torneos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    torneo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo_competencia = table.Column<string>(type: "text", nullable: false),
                    sexo = table.Column<string>(type: "text", nullable: false),
                    rango_edad_min = table.Column<int>(type: "integer", nullable: false),
                    rango_edad_max = table.Column<int>(type: "integer", nullable: false),
                    rango_peso_min = table.Column<decimal>(type: "numeric", nullable: false),
                    rango_peso_max = table.Column<decimal>(type: "numeric", nullable: false),
                    rango_graduacion_min = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    rango_graduacion_max = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categorias", x => x.id);
                    table.ForeignKey(
                        name: "fk_categorias_torneos_torneo_id",
                        column: x => x.torneo_id,
                        principalTable: "torneos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "competidores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    torneo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    apellido = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    edad = table.Column<int>(type: "integer", nullable: false),
                    graduacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    peso = table.Column<decimal>(type: "numeric", nullable: false),
                    altura = table.Column<decimal>(type: "numeric", nullable: false),
                    escuela = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    responsable = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_competidores", x => x.id);
                    table.ForeignKey(
                        name: "fk_competidores_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_competidores_torneos_torneo_id",
                        column: x => x.torneo_id,
                        principalTable: "torneos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "llaves_competencia",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ronda = table.Column<int>(type: "integer", nullable: false),
                    posicion = table.Column<int>(type: "integer", nullable: false),
                    competidor1id = table.Column<Guid>(type: "uuid", nullable: true),
                    competidor2id = table.Column<Guid>(type: "uuid", nullable: true),
                    ganador_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_llaves_competencia", x => x.id);
                    table.ForeignKey(
                        name: "fk_llaves_competencia_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_llaves_competencia_competidores_competidor1id",
                        column: x => x.competidor1id,
                        principalTable: "competidores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_llaves_competencia_competidores_competidor2id",
                        column: x => x.competidor2id,
                        principalTable: "competidores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_llaves_competencia_competidores_ganador_id",
                        column: x => x.ganador_id,
                        principalTable: "competidores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_categorias_torneo_id",
                table: "categorias",
                column: "torneo_id");

            migrationBuilder.CreateIndex(
                name: "ix_competidores_categoria_id",
                table: "competidores",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_competidores_torneo_id",
                table: "competidores",
                column: "torneo_id");

            migrationBuilder.CreateIndex(
                name: "ix_llaves_competencia_categoria_id",
                table: "llaves_competencia",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_llaves_competencia_competidor1id",
                table: "llaves_competencia",
                column: "competidor1id");

            migrationBuilder.CreateIndex(
                name: "ix_llaves_competencia_competidor2id",
                table: "llaves_competencia",
                column: "competidor2id");

            migrationBuilder.CreateIndex(
                name: "ix_llaves_competencia_ganador_id",
                table: "llaves_competencia",
                column: "ganador_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "llaves_competencia");

            migrationBuilder.DropTable(
                name: "competidores");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "torneos");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarteraVirtual.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pdt_users_groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Permisos = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_users_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pdt_users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grupo = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pdt_users_pdt_users_groups_Grupo",
                        column: x => x.Grupo,
                        principalTable: "pdt_users_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdt_crtr_gastos_tipo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Propietario = table.Column<int>(type: "int", nullable: false),
                    presupuesto_col = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    presupuesto_usd = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_crtr_gastos_tipo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pdt_crtr_gastos_tipo_pdt_users_Propietario",
                        column: x => x.Propietario,
                        principalTable: "pdt_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdt_fondos_monetarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    capitalCOP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    capitalUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Propietario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_fondos_monetarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pdt_fondos_monetarios_pdt_users_Propietario",
                        column: x => x.Propietario,
                        principalTable: "pdt_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdt_presupuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    GastoTipoId = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    MontoPresupuestado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_presupuesto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pdt_presupuesto_pdt_crtr_gastos_tipo_GastoTipoId",
                        column: x => x.GastoTipoId,
                        principalTable: "pdt_crtr_gastos_tipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pdt_presupuesto_pdt_users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "pdt_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdt_depositos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_depositos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pdt_depositos_pdt_fondos_monetarios_FondoId",
                        column: x => x.FondoId,
                        principalTable: "pdt_fondos_monetarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdt_gastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoMonetario = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Criterio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    propietario = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_gastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pdt_gastos_pdt_fondos_monetarios_FondoMonetario",
                        column: x => x.FondoMonetario,
                        principalTable: "pdt_fondos_monetarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdt_gastos_detalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GastoRegistroId = table.Column<int>(type: "int", nullable: false),
                    TipoGasto = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdt_gastos_detalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pdt_gastos_detalle_pdt_crtr_gastos_tipo_TipoGasto",
                        column: x => x.TipoGasto,
                        principalTable: "pdt_crtr_gastos_tipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pdt_gastos_detalle_pdt_gastos_GastoRegistroId",
                        column: x => x.GastoRegistroId,
                        principalTable: "pdt_gastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pdt_crtr_gastos_tipo_Propietario",
                table: "pdt_crtr_gastos_tipo",
                column: "Propietario");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_depositos_FondoId",
                table: "pdt_depositos",
                column: "FondoId");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_fondos_monetarios_Propietario",
                table: "pdt_fondos_monetarios",
                column: "Propietario");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_gastos_FondoMonetario",
                table: "pdt_gastos",
                column: "FondoMonetario");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_gastos_detalle_GastoRegistroId",
                table: "pdt_gastos_detalle",
                column: "GastoRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_gastos_detalle_TipoGasto",
                table: "pdt_gastos_detalle",
                column: "TipoGasto");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_presupuesto_GastoTipoId",
                table: "pdt_presupuesto",
                column: "GastoTipoId");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_presupuesto_UsuarioId",
                table: "pdt_presupuesto",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_pdt_users_Grupo",
                table: "pdt_users",
                column: "Grupo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pdt_depositos");

            migrationBuilder.DropTable(
                name: "pdt_gastos_detalle");

            migrationBuilder.DropTable(
                name: "pdt_presupuesto");

            migrationBuilder.DropTable(
                name: "pdt_gastos");

            migrationBuilder.DropTable(
                name: "pdt_crtr_gastos_tipo");

            migrationBuilder.DropTable(
                name: "pdt_fondos_monetarios");

            migrationBuilder.DropTable(
                name: "pdt_users");

            migrationBuilder.DropTable(
                name: "pdt_users_groups");
        }
    }
}

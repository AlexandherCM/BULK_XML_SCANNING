using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BULK_DOWNLOAD_CFDI.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CtasFacturas",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Version = table.Column<float>(type: "real", nullable: false),
                    Emitida = table.Column<bool>(type: "bit", nullable: false),
                    UUID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FolioInterno = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Folio = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SerieInterna = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RFC_Emisor = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Nombre_Emisor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RegimenEmisor = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    RFC_Receptor = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Nombre_Receptor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RegimenReceptor = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CP_Expedicion = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    TipoComprobante = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    subtotal_MXN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    descuento_MXN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total_MXN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Condiciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UsoCFDI = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    IVA_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVA_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCobroPago = table.Column<long>(type: "bigint", nullable: false),
                    FechaCargado = table.Column<long>(type: "bigint", nullable: false),
                    FechaCreacion = table.Column<long>(type: "bigint", nullable: false),
                    FechaTimbrado = table.Column<long>(type: "bigint", nullable: false),
                    Expirada = table.Column<bool>(type: "bit", nullable: false),
                    Timbrada = table.Column<bool>(type: "bit", nullable: false),
                    Estatus = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CtasFacturas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CtasConceptos",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FacturaID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ClaveProductoServicio = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    ClaveUnidad = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVA_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVA_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CtasConceptos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CtasConceptos_CtasFacturas_FacturaID",
                        column: x => x.FacturaID,
                        principalTable: "CtasFacturas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CtasPagosFactura",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FacturaPagoID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    NoPago = table.Column<int>(type: "int", nullable: false),
                    NumOperacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVA_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVA_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CtasPagosFactura", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CtasPagosFactura_CtasFacturas_FacturaPagoID",
                        column: x => x.FacturaPagoID,
                        principalTable: "CtasFacturas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CtasDocsRelacionados",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    PagoID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FacturaPPD_UUID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FacturaPPD_ID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    NumParcialidad = table.Column<int>(type: "int", nullable: false),
                    ImpSaldoInsoluto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImpSaldoAnt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImpPagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVA_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVA_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IEPS_Trasladado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Contemplado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CtasDocsRelacionados", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CtasDocsRelacionados_CtasPagosFactura_PagoID",
                        column: x => x.PagoID,
                        principalTable: "CtasPagosFactura",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CtasConceptos_FacturaID",
                table: "CtasConceptos",
                column: "FacturaID");

            migrationBuilder.CreateIndex(
                name: "IX_CtasDocsRelacionados_PagoID",
                table: "CtasDocsRelacionados",
                column: "PagoID");

            migrationBuilder.CreateIndex(
                name: "IX_CtasPagosFactura_FacturaPagoID",
                table: "CtasPagosFactura",
                column: "FacturaPagoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CtasConceptos");

            migrationBuilder.DropTable(
                name: "CtasDocsRelacionados");

            migrationBuilder.DropTable(
                name: "CtasPagosFactura");

            migrationBuilder.DropTable(
                name: "CtasFacturas");
        }
    }
}

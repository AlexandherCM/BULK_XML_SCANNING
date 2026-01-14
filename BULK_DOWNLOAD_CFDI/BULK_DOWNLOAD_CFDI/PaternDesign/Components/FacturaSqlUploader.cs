using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BULK_DOWNLOAD_CFDI.PaternDesign.DTOs;

namespace BULK_DOWNLOAD_CFDI.PaternDesign.Components
{
    public class FacturaSqlUploader
    {
        private readonly IConfiguration _configuration;

        public FacturaSqlUploader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SubirFacturasAsync(List<FacturaDataRecordOrderDTO> registros)
        {
            const int batchSize = 1000;
            var conn = _configuration.GetConnectionString("Context");

            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            //CARGA DE DATOS POR LOTES, CADA QUE i SEA MENOR SE SUMA 1,000 AL INDICE PARA CARGAR
            //EL CONJUNTO DE MIL EN CURSO
            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                var batchFacturas = new List<SqlDataRecord>();
                var batchConceptos = new List<SqlDataRecord>();
                var batchPagos = new List<SqlDataRecord>();
                var batchRelacionados = new List<SqlDataRecord>();

                foreach (var item in batch)
                {
                    batchFacturas.Add(item.Factura);

                    if (item.Conceptos != null && item.Conceptos.Any())
                        batchConceptos.AddRange(item.Conceptos);

                    if (item.PagosYDtosRelacionados != null && item.PagosYDtosRelacionados.Any())
                    {
                        foreach (var pagoDto in item.PagosYDtosRelacionados)
                        {
                            batchPagos.Add(pagoDto.Pago);
                            if (pagoDto.Relacionados != null && pagoDto.Relacionados.Any())
                                batchRelacionados.AddRange(pagoDto.Relacionados);
                        }
                    }
                }

                using var command = new SqlCommand("sp_ProcesarFacturasCompleto", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 300
                };

                object GetOrNull(List<SqlDataRecord> r) => r.Any() ? r : null;

                command.Parameters.Add(new SqlParameter("@Facturas", SqlDbType.Structured)
                {
                    TypeName = "dbo.CtasFacturasDTO",
                    Value = GetOrNull(batchFacturas)
                });
                command.Parameters.Add(new SqlParameter("@Conceptos", SqlDbType.Structured)
                {
                    TypeName = "dbo.CtasConceptosDTO",
                    Value = GetOrNull(batchConceptos)
                });
                command.Parameters.Add(new SqlParameter("@Pagos", SqlDbType.Structured)
                {
                    TypeName = "dbo.CtasPagosFacturaDTO",
                    Value = GetOrNull(batchPagos)
                });
                command.Parameters.Add(new SqlParameter("@Relacionados", SqlDbType.Structured)
                {
                    TypeName = "dbo.CtasDocsRelacionadosDTO",
                    Value = GetOrNull(batchRelacionados)
                });

                await command.ExecuteNonQueryAsync();
            }

            // Ejecutar SP final para actualizar montos y marcar relacionados
            using var finalCommand = new SqlCommand("sp_ActualizarMontosYRelacionados", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await finalCommand.ExecuteNonQueryAsync();
        }
    }



}
